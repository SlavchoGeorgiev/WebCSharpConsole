using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    public class ConsoleApplicationEmulator : IConsoleApplicationEmulator
    {
        private const string CompilationFolderName = "TempCompilation";
        private const string ConsoleCompilationPrefix = "ConsoleApp_";
        private const int DefaultMaxConsoleApplicationExecutionTimeMs = 10 * 1000;
        private static readonly List<MetadataReference> defaultReferences = new List<MetadataReference>
            {
               MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
               MetadataReference.CreateFromFile(typeof(System.Linq.IQueryable).Assembly.Location),
            };
        private readonly long maxConsoleApplicationExecutionTimeMs;
        private readonly List<MetadataReference> references;
        private readonly string currentExecutionAssemblyFolderPath;
        private readonly Assembly dummyConsoleAssembly;
        private readonly Lazy<string> tempCompilationPath;
        private readonly CSharpCompilationOptions compilationOptions;
        private readonly string assemblyLocation;
        private AppDomain appDomain;
        private bool isAssemblyGenerated;

        public ConsoleApplicationEmulator() : this(DefaultMaxConsoleApplicationExecutionTimeMs)
        {
        }

        public ConsoleApplicationEmulator(long maxConsoleApplicationExecutionTimeMs)
        {
            this.maxConsoleApplicationExecutionTimeMs = maxConsoleApplicationExecutionTimeMs;
            this.dummyConsoleAssembly = typeof(DummyConsole.Console).Assembly;
            this.references = new List<MetadataReference>();
            this.references.AddRange(defaultReferences);
            this.references.Add(MetadataReference.CreateFromFile(this.dummyConsoleAssembly.Location));

            string currentExecutionAssemblyPath = Assembly.GetExecutingAssembly().Location;
            this.currentExecutionAssemblyFolderPath = Path.GetDirectoryName(currentExecutionAssemblyPath);
            this.tempCompilationPath = new Lazy<string>(() =>
            {
                string tempCompilationPath = Path.Combine(this.currentExecutionAssemblyFolderPath, CompilationFolderName);
                if (!Directory.Exists(tempCompilationPath))
                {
                    Directory.CreateDirectory(tempCompilationPath);
                }

                return tempCompilationPath;
            });

            var currentAssembly = this.GetType().Assembly;
            this.MoveAssembliesToTempCompilationDirectory(this.TempCompilationPath, new Assembly[] { this.dummyConsoleAssembly, currentAssembly });
            this.compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            this.CompilationId = Guid.NewGuid();
            this.assemblyLocation = this.GetNewCompilationAssemblyLocation(this.ConsoleAssemblyName + ".exe");
            this.appDomain = this.CreateSandbox();
        }

        public Guid CompilationId { get; private set; }

        public long MaxConsoleApplicationExecutionTimeMs => this.maxConsoleApplicationExecutionTimeMs;

        private string TempCompilationPath => this.tempCompilationPath.Value;

        private string ConsoleAssemblyName => $"{ConsoleCompilationPrefix}{this.CompilationId}";

        public EmitResult Compile(string code)
        {
            if (this.isAssemblyGenerated)
            {
                throw new InvalidOperationException($"Compilation already exists, please invoke method {nameof(this.Recycle)}.");
            }

            CSharpCompilation compilation = this.CompileAndMockConsole(code);
            EmitResult result = compilation.Emit(this.assemblyLocation);

            if (result.Success)
            {
                this.isAssemblyGenerated = true;

                return result;
            }

            return result;
        }

        public EmitResult TestCompile(string code)
        {
            var compilation = this.CompileCore(code);
            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            return emitResult;
        }

        public async Task<ConsoleExecutionResult> RunAsync()
        {
            if (!this.isAssemblyGenerated)
            {
                throw new InvalidOperationException($@"Compilation is not available please invoke {nameof(this.Compile)} method and assert that compilation is successful.");
            }

            var totalExecutionTimmer = Stopwatch.StartNew();
            Task<ConsoleExecutionResult> resultTask = this.ExecuteInAssemblyInAppDomain();

            while (!resultTask.IsCompleted)
            {
                await Task.Delay(100);
                if (totalExecutionTimmer.ElapsedMilliseconds <= this.maxConsoleApplicationExecutionTimeMs)
                {
                    continue;
                }

                var result = new ConsoleExecutionResult()
                {
                    IsTimeouted = true
                };

                return result;
            }

            return resultTask.Result;
        }

        public void Recycle()
        {
            this.isAssemblyGenerated = false;
            this.CleanUp();
            this.appDomain = this.CreateSandbox();
        }

        public IEnumerable<ISymbol> GetRecommendedSymblos(string code, int index)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var syntaxTrees = new[] { syntaxTree };
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName: this.ConsoleAssemblyName,
                                                                    syntaxTrees: syntaxTrees,
                                                                    references: this.references.Where(r => r.Display != this.dummyConsoleAssembly.Location),
                                                                    options: this.compilationOptions);
            var workspace = new AdhocWorkspace();
            string projName = "NewProject";
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, projName, projName, LanguageNames.CSharp);
            var newProject = workspace.AddProject(projectInfo);
            var sourceText = SourceText.From(code);
            var newDocument = workspace.AddDocument(newProject.Id, "NewFile.cs", sourceText);
            var recommendedSymblos = Recommender.GetRecommendedSymbolsAtPositionAsync(compilation.GetSemanticModel(syntaxTree), index, workspace).Result;

            return recommendedSymblos;
        }

        public void Dispose()
        {
            this.CleanUp();
        }

        private Task<ConsoleExecutionResult> ExecuteInAssemblyInAppDomain()
        {
            return Task.Run(() =>
            {
                var consoleExecutor = this.GetConsoleExecutor(this.appDomain);
                var consoleExecuteResult = new ConsoleExecutionResult();
                try
                {
                    var executionTimmer = Stopwatch.StartNew();
                    consoleExecuteResult.ConsoleOutput = consoleExecutor.Execute();
                    consoleExecuteResult.ExecutionTimeMs = executionTimmer.ElapsedMilliseconds;
                    consoleExecuteResult.Success = true;

                    return consoleExecuteResult;
                }
                catch (Exception ex)
                {
                    consoleExecuteResult.IsExceptionThrown = true;
                    consoleExecuteResult.Exception = ex.InnerException;

                    return consoleExecuteResult;
                }
            });
        }

        private void CleanUp()
        {
            AppDomain.Unload(this.appDomain);
            GC.Collect(); // collects all unused memory
            GC.WaitForPendingFinalizers(); // wait until GC has finished its work
            GC.Collect();
            File.Delete(this.assemblyLocation);
        }

        private CSharpCompilation CompileAndMockConsole(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var syntaxTrees = new[] { syntaxTree };
            CSharpCompilation compilation = this.CreateCompilation(syntaxTrees);

            var visitor = new SystemConsoleRewriter(compilation.GetSemanticModel(syntaxTree));
            var newSyntaxTree = visitor.Visit(syntaxTree.GetRoot()).NormalizeWhitespace().SyntaxTree;
            var newSyntaxTrees = new[] { newSyntaxTree };
            CSharpCompilation newCompilationWithMockedConsole = this.CreateCompilation(newSyntaxTrees);

            return newCompilationWithMockedConsole;
        }

        private CSharpCompilation CompileCore(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var syntaxTrees = new[] { syntaxTree };
            CSharpCompilation compilation = this.CreateCompilation(syntaxTrees);

            return compilation;
        }

        private CSharpCompilation CreateCompilation(SyntaxTree[] syntaxTrees)
        {
            return CSharpCompilation.Create(assemblyName: this.ConsoleAssemblyName,
                                            syntaxTrees: syntaxTrees,
                                            references: this.references,
                                            options: this.compilationOptions);
        }

        private ConsoleExecutor GetConsoleExecutor(AppDomain appDomain)
        {
            var consoleExecutor = (ConsoleExecutor)appDomain.CreateInstanceAndUnwrap(typeof(ConsoleExecutor).Assembly.FullName,
                                                                                     typeof(ConsoleExecutor).FullName);
            consoleExecutor.SetAssembly(this.assemblyLocation);

            return consoleExecutor;
        }

        private AppDomain CreateSandbox()
        {
            return this.CreateSandbox($"Console Application emulator app domain: {Guid.NewGuid()}",
                 Path.GetDirectoryName(this.assemblyLocation),
                 new string[]
                 {
                    this.dummyConsoleAssembly.Location,
                    this.assemblyLocation,
                    //this.currentTempCompilationAssembly.Location
                 });
        }

        private AppDomain CreateSandbox(string appDomainName, string rootPath, string[] assemblyPaths)
        {
            var setup = new AppDomainSetup()
            {
                ApplicationBase = rootPath ?? AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            };

            Evidence baseEvidence = AppDomain.CurrentDomain.Evidence;
            Evidence evidence = new Evidence(baseEvidence);

            var permissionSet = new PermissionSet(PermissionState.None);
            //permissionSet.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissionSet.AddPermission(new UIPermission(UIPermissionWindow.AllWindows, UIPermissionClipboard.AllClipboard));

            var fileIoPermission = new FileIOPermission(FileIOPermissionAccess.Read, assemblyPaths);
            fileIoPermission.AddPathList(FileIOPermissionAccess.PathDiscovery, assemblyPaths);
            permissionSet.AddPermission(fileIoPermission);

            var appDomain = AppDomain.CreateDomain(appDomainName, evidence, setup, permissionSet);

            return appDomain;
        }

        private string GetNewCompilationAssemblyLocation(string newAssemblyFileName)
        {
            var tempCompilationPath = this.TempCompilationPath;
            return Path.Combine(tempCompilationPath, newAssemblyFileName);
        }

        private void MoveAssembliesToTempCompilationDirectory(string tempCompilationDirectory, Assembly[] assemblies)
        {
            foreach (var currentAssembly in assemblies)
            {
                var currentAssemblyLocation = currentAssembly.Location;
                var currentassemblyFileName = Path.GetFileName(currentAssemblyLocation);
                var newAssemblyLocation = Path.Combine(tempCompilationDirectory, currentassemblyFileName);
                if (!File.Exists(newAssemblyLocation))
                {
                    File.Copy(currentAssemblyLocation, newAssemblyLocation);
                }
            }
        }
    }
}

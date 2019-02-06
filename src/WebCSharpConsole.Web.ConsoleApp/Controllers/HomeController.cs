using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCSharpConsole.Services.ConsoleEmulator;
using WebCSharpConsole.Web.ConsoleApp.Extensions;
using WebCSharpConsole.Web.ConsoleApp.ViewModels.Home;
using WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.Recomendations;
using WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.TestCompile;

namespace WebCSharpConsole.Web.ConsoleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConsoleApplicationEmulator consoleApplicationEmulator;

        public HomeController(IConsoleApplicationEmulator consoleApplicationEmulator)
        {
            this.consoleApplicationEmulator = consoleApplicationEmulator;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult GetRecommendations(string code, int index)
        {
            var recommendations = this.consoleApplicationEmulator.GetRecommendedSymblos(code, index);

            var completionItems = recommendations
            .Select(r => new CompletionItemViewModel()
            {
                Label = r.Name,
                Kind = r.ToCompletionItemKind(),
                Documentation = r.ToCompletionItemDocumentation(),
                InsertText = r.Name,
                Detail = r.ToCompletionItemDetails()
            })
            .GroupBy(r => new { r.Label, r.Kind, r.Detail })
            .Select(rg => new CompletionItemViewModel()
            {
                Label = rg.Key.Label,
                Kind = rg.Key.Kind,
                InsertText = rg.Key.Label,
                Detail = rg.Key.Detail,
                Documentation = string.Join(Environment.NewLine, rg.Select(r => r.Documentation))
            })
            .OrderBy(r => r.Label)
            .ThenBy(r => r.Kind)
            .ToList();

            var keywords = this.GetCSharpKeywords();
            var result = keywords;
            result.AddRange(completionItems);

            return this.Ok(result);
        }

        public IActionResult TestCompile(string code)
        {
            var compilationResult = this.consoleApplicationEmulator.TestCompile(code);
            if (compilationResult.Success)
            {
                return this.Ok(new { Success = true });
            }

            return this.Ok(new TestCompileViewModel()
            {
                Success = false,
                Diagnostics = compilationResult.Diagnostics
                .Select(d =>
                {
                    var lineSpan = d.Location.GetLineSpan();

                    var startLineNumber = lineSpan.StartLinePosition.Line + 1;
                    var startColumnNumber = lineSpan.StartLinePosition.Character + 1;
                    var endLineNumber = lineSpan.EndLinePosition.Line + 1;
                    var endColumnNumber = lineSpan.EndLinePosition.Character + 1;

                    if (startColumnNumber == endColumnNumber)
                    {
                        startColumnNumber--;
                    }

                    var diagnostic = new DiagnosticViewModel()
                    {
                        DiagnosticKind = d.DefaultSeverity,
                        Message = $"{d.DefaultSeverity}: {d.GetMessage()}",
                        Range = new RangeViewModel()
                        {
                            StartLineNumber = startLineNumber,
                            StartColumnNumber = startColumnNumber,
                            EndLineNumber = endLineNumber,
                            EndColumnNumber = endColumnNumber,
                        }
                    };

                    return diagnostic;
                })
            });
        }

        public async Task<IActionResult> CompileAndRun(string code)
        {
            var compilationResult = this.consoleApplicationEmulator.Compile(code);
            if (!compilationResult.Success)
            {
                return this.Ok(new CompileAndRunResponseModel()
                {
                    CompilationFailed = true,
                    CompilationErrors = string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(d => d.ToReadableErrorText()))
                });
            }

            var runResult = await this.consoleApplicationEmulator.RunAsync();

            if (!runResult.Success)
            {
                if (runResult.IsTimeouted)
                {
                    return this.Ok(new CompileAndRunResponseModel()
                    {
                        IsTimeouted = true,
                        MaxExecutionTimeMs = this.consoleApplicationEmulator.MaxConsoleApplicationExecutionTimeMs
                    });
                }

                if (runResult.IsExceptionThrown)
                {
                    return this.Ok(new CompileAndRunResponseModel()
                    {
                        IsExceptionThrown = true,
                        ExceptionMessage = runResult.Exception.Message,
                        ExceptionStackTrace = runResult.Exception.StackTrace
                    });
                }
            }

            return this.Ok(new CompileAndRunResponseModel()
            {
                Success = true,
                ConsoleOutput = runResult.ConsoleOutput,
                ExecutionTimeMs = runResult.ExecutionTimeMs
            });
        }

        public IActionResult Error()
        {
            return this.View();
        }

        private List<CompletionItemViewModel> GetCSharpKeywords()
        {
            return LanguageKeywords.CSharpKeywords.Select(kw => new CompletionItemViewModel()
            {
                Label = kw,
                Kind = CompletionItemKind.Keyword,
                InsertText = kw
            })
                                                  .OrderBy(ci => ci.Label)
                                                  .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using WebCSharpConsole.Services.ConsoleEmulator;

namespace WebCSharpConsole.Services.ConsoleEmulatorTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ConsoleApplicationEmulatorTests
    {
        private const string ConsoleAppTemplate = @"
using System;
using static System.Console;
using System.Threading;

namespace ConsoleApplication
{{
    public class StartUp
    {{
        public static void Main()
        {{
            {0}
        }}
    }}
}}";

        [Test]
        public void ConsoleApplicationWriteString_ShouldReturn_CorrectResult()
        {
            var consoleText = "Hi";
            var codeSnipet = $@"
            Console.Write(""{consoleText}"");
            Console.WriteLine();
            WriteLine();
            Action actionWriteLine = WriteLine;
            actionWriteLine();
            Action<string> funcWriteLine = Console.WriteLine;
            funcWriteLine(""{consoleText}"");";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var newLine = Environment.NewLine;
            var expectedOutput = $"{consoleText}{newLine}{newLine}{newLine}{consoleText}{newLine}";
            var executionResult = CompileAndRun(code);

            Assert.IsTrue(executionResult.Success);
            Assert.AreEqual(expectedOutput, executionResult.ConsoleOutput);
        }

        [Test]
        public void ConsoleApplicationMultipleExecutionAndRecycling_ShouldProvide_SameResult()
        {
            var expectedOutput = "Hi";
            var codeSnipet = $@"Console.Write(""{expectedOutput}"");";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            List<string> executionResults = new List<string>();

            using (var compiler = new ConsoleApplicationEmulator())
            {
                for (int i = 0; i < 50; i++)
                {
                    compiler.Compile(code);
                    var result = compiler.RunAsync().Result;
                    Assert.IsTrue(result.Success);
                    executionResults.Add(result.ConsoleOutput);
                    result = compiler.RunAsync().Result;
                    compiler.Recycle();
                    Assert.IsTrue(result.Success);
                    executionResults.Add(result.ConsoleOutput);
                }
            }

            Assert.IsTrue(executionResults.All(actual => actual == expectedOutput));
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithExpectedException()
        {
            var expectedExceptionType = typeof(InvalidOperationException);
            var expectedExceptionMessage = "InvalidOperationException";
            var codeSnippet = $@"throw new InvalidOperationException(""{expectedExceptionMessage}"");";
            var code = string.Format(ConsoleAppTemplate, codeSnippet);

            var executionResult = CompileAndRun(code);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsExceptionThrown);
            Assert.AreEqual(expectedExceptionType, executionResult?.Exception.GetType());
            Assert.AreEqual(expectedExceptionMessage, executionResult?.Exception.Message);
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithTimeout_WithThreadSleep()
        {
            var codeSnipet = $@"Thread.Sleep(10000);";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code, 1000);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsTimeouted);
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithTimeout_WithConsoleRead()
        {
            var codeSnipet = $@"Console.Read();";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code, 1000);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsTimeouted);
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithAmbiguousMatchException()
        {
            var expectedExceptionType = typeof(AmbiguousMatchException);
            var expectedExceptionMessage = "Ambiguous match found.";
            var codeSnipet = $@"typeof(Console).GetMethod(""Write""); // Must throw Exception";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsExceptionThrown);
            Assert.AreEqual(expectedExceptionType, executionResult?.Exception.GetType());
            Assert.AreEqual(expectedExceptionMessage, executionResult?.Exception.Message);
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithSecurityException()
        {
            var expectedExceptionType = typeof(System.Security.SecurityException);
            var codeSnipet = $@"
            var ms = new System.IO.MemoryStream();
            var sw = new System.IO.StreamWriter(ms);
            Console.SetOut(sw);";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsExceptionThrown);
            Assert.AreEqual(expectedExceptionType, executionResult?.Exception.GetType());
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithSecurityExceptionAccessingConsoleIn()
        {
            var expectedExceptionType = typeof(System.Security.SecurityException);
            var codeSnipet = $@"Console.In.Read();";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsExceptionThrown);
            Assert.AreEqual(expectedExceptionType, executionResult?.Exception.GetType());
        }

        [Test]
        public void ConsoleApplication_ShouldReturn_UnsuccessfulResultWithSecurityExceptionAccessingConsoleReadKye()
        {
            var expectedExceptionType = typeof(System.Security.SecurityException);
            var codeSnipet = $@"Console.ReadKey(true);";
            var code = string.Format(ConsoleAppTemplate, codeSnipet);

            var executionResult = CompileAndRun(code);

            Assert.IsFalse(executionResult.Success);
            Assert.IsTrue(executionResult.IsExceptionThrown);
            Assert.AreEqual(expectedExceptionType, executionResult?.Exception.GetType());
        }

        private static ConsoleExecutionResult CompileAndRun(string code, long maxExecutionTimeMs = long.MaxValue)
        {
            ConsoleExecutionResult executionResult;
            using (var compiler = new ConsoleApplicationEmulator(maxExecutionTimeMs))
            {
                compiler.Compile(code);
                executionResult = compiler.RunAsync().Result;
            }

            return executionResult;
        }
    }
}

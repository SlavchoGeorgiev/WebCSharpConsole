using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    public interface IConsoleApplicationEmulator : IDisposable
    {
        Guid CompilationId { get; }

        long MaxConsoleApplicationExecutionTimeMs { get; }

        EmitResult Compile(string code);

        EmitResult TestCompile(string code);

        Task<ConsoleExecutionResult> RunAsync();

        IEnumerable<ISymbol> GetRecommendedSymblos(string code, int index);

        void Recycle();
    }
}

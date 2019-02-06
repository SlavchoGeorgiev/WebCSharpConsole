using Microsoft.CodeAnalysis;

namespace WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.TestCompile
{
    public class DiagnosticViewModel
    {
        public DiagnosticSeverity DiagnosticKind { get; set; }

        public string Message { get; set; }

        public RangeViewModel Range { get; set; }
    }
}

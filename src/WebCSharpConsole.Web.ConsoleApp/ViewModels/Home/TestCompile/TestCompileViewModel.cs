using System.Collections.Generic;

namespace WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.TestCompile
{
    public class TestCompileViewModel
    {
        public bool Success { get; set; }

        public IEnumerable<DiagnosticViewModel> Diagnostics { get; set; }
    }
}

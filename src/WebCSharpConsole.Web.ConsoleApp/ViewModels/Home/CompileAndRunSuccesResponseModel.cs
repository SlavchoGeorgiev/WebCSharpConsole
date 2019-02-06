namespace WebCSharpConsole.Web.ConsoleApp.ViewModels.Home
{
    public class CompileAndRunResponseModel
    {
        public bool Success { get; set; }

        public bool CompilationFailed { get; set; }

        public bool IsTimeouted { get; set; }

        public bool IsExceptionThrown { get; set; }

        public string ConsoleOutput { get; set; }

        public long ExecutionTimeMs { get; set; }

        public string CompilationErrors { get; set; }

        public long MaxExecutionTimeMs { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }
    }
}

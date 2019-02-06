using System;

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    [Serializable]
    public class ConsoleExecutionResult : MarshalByRefObject
    {
        public bool Success { get; set; }

        public string ConsoleOutput { get; set; }

        public bool IsTimeouted { get; set; }

        public bool IsExceptionThrown { get; set; }

        public Exception Exception { get; set; }

        public long ExecutionTimeMs { get; set; }
    }
}

namespace WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.Recomendations
{
    public class Range
    {
        /// <summary>
        /// Column on which the range starts in line startLineNumber (starts at 1).
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// Column on which the range ends in line endLineNumber.
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// Line number on which the range starts (starts at 1).
        /// </summary>
        public int StartLineNumber { get; set; }

        /// <summary>
        /// Line number on which the range ends.
        /// </summary>
        public int EndLineNumber { get; set; }
    }
}

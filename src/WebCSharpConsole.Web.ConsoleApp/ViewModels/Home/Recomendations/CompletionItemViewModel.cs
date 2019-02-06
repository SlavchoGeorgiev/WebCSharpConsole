namespace WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.Recomendations
{
    public class CompletionItemViewModel
    {
        /// <summary>
        /// The label of this completion item. By default this is also the text that is inserted when selecting this completion.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The kind of this completion item. Based on the kind an icon is chosen by the editor.
        /// </summary>
        public CompletionItemKind Kind { get; set; }

        /// <summary>
        /// A human-readable string with additional information about this item, like type or symbol information.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// A human-readable string that represents a doc-comment.
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// A string that should be used when filtering a set of completion items. When falsy the label is used.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// A string or snippet that should be inserted in a document when selecting this completion. When falsy the label is used.
        /// </summary>
        public string InsertText { get; set; }

        /// <summary>
        /// A range of text that should be replaced by this completion item.
        /// Defaults to a range from the start of the current word to the current position.
        /// Note: The range must be a single line and it must contain the position at which completion has been requested.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// A string that should be used when comparing this item with other items. When falsy the label is used.
        /// </summary>
        public string SortText { get; set; }
    }
}

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    public class LanguageKeywords
    {
        private static readonly string[] cSharpKeywords = new string[]
            {
                "extern", "alias", "using", "bool", "decimal", "sbyte", "byte", "short",
                "ushort", "int", "uint", "long", "ulong", "char", "float", "double",
                "object", "dynamic", "string", "assembly", "is", "as", "ref",
                "out", "this", "base", "new", "typeof", "void", "checked", "unchecked",
                "default", "delegate", "var", "const", "if", "else", "switch", "case",
                "while", "do", "for", "foreach", "in", "break", "continue", "goto",
                "return", "throw", "try", "catch", "finally", "lock", "yield", "from",
                "let", "where", "join", "on", "equals", "into", "orderby", "ascending",
                "descending", "select", "group", "by", "namespace", "partial", "class",
                "field", "event", "method", "param", "property", "public", "protected",
                "internal", "private", "abstract", "sealed", "static", "struct", "readonly",
                "volatile", "virtual", "override", "params", "get", "set", "add", "remove",
                "operator", "true", "false", "implicit", "explicit", "interface", "enum",
                "null", "async", "await", "fixed", "sizeof", "stackalloc", "unsafe", "nameof",
                "when"
            };

        public static string[] CSharpKeywords => cSharpKeywords;
    }
}

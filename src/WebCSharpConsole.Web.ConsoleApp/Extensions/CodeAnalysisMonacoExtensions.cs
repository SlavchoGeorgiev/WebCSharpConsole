using System.Text;
using Microsoft.CodeAnalysis;
using WebCSharpConsole.Web.ConsoleApp.ViewModels.Home.Recomendations;

namespace WebCSharpConsole.Web.ConsoleApp.Extensions
{
    public static class CodeAnalysisMonacoExtensions
    {
        public static CompletionItemKind ToCompletionItemKind(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Alias:
                case SymbolKind.Assembly:
                    return CompletionItemKind.Folder;
                case SymbolKind.Namespace:
                    return CompletionItemKind.Module;
                case SymbolKind.Property:
                    return CompletionItemKind.Property;
                case SymbolKind.Event:
                    return CompletionItemKind.Reference;
                case SymbolKind.Field:
                    return CompletionItemKind.Field;
                case SymbolKind.Parameter:
                case SymbolKind.RangeVariable:
                case SymbolKind.Local:
                case SymbolKind.Label:
                    return CompletionItemKind.Variable;
                case SymbolKind.Method:
                    return CompletionItemKind.Method;
                case SymbolKind.NetModule:
                    return CompletionItemKind.Module;
                case SymbolKind.DynamicType:
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                case SymbolKind.TypeParameter:
                case SymbolKind.PointerType:
                    var namedTypeSymbol = (INamedTypeSymbol)symbol;

                    switch (namedTypeSymbol.TypeKind)
                    {
                        case TypeKind.Array:
                        case TypeKind.Class:
                        case TypeKind.Delegate:
                        case TypeKind.Dynamic:
                        case TypeKind.Submission:
                        case TypeKind.Error:
                            return CompletionItemKind.Class;
                        case TypeKind.Struct:
                            return CompletionItemKind.File;
                        case TypeKind.Enum:
                            return CompletionItemKind.Enum;
                        case TypeKind.Interface:
                            return CompletionItemKind.Interface;
                        case TypeKind.Module:
                            return CompletionItemKind.Module;
                        case TypeKind.Pointer:
                            return CompletionItemKind.Unit;
                        case TypeKind.TypeParameter:
                            return CompletionItemKind.Variable;
                        case TypeKind.Unknown:
                            return CompletionItemKind.Snippet;
                        default:
                            return CompletionItemKind.Snippet;
                    }
                case SymbolKind.Preprocessing:
                case SymbolKind.Discard:
                    return CompletionItemKind.Snippet;
                default:
                    return CompletionItemKind.Snippet;
            }
        }

        public static string ToCompletionItemDetails(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.DynamicType:
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                case SymbolKind.TypeParameter:
                case SymbolKind.PointerType:
                    var namedTypeSymbol = (ITypeSymbol)symbol;
                    return namedTypeSymbol.TypeKind.ToString();
                default:
                    return symbol.Kind.ToString();
            }
        }

        public static string ToCompletionItemDocumentation(this ISymbol symbol)
        {
            var sb = new StringBuilder();
            var documentationCommentXml = symbol.GetDocumentationCommentXml();
            if (!string.IsNullOrEmpty(documentationCommentXml))
            {
                sb.AppendLine(documentationCommentXml);
            }

            var documentationCommentId = symbol.GetDocumentationCommentId();
            if (!string.IsNullOrEmpty(documentationCommentId))
            {
                sb.AppendLine(documentationCommentId);
            }

            sb.AppendLine(symbol.OriginalDefinition.ToString());

            return sb.ToString();
        }

        public static string ToReadableErrorText(this Diagnostic diagnostic)
        {
            if (diagnostic.DefaultSeverity != DiagnosticSeverity.Error || diagnostic.IsWarningAsError)
            {
                return string.Empty;
            }

            const string Separator = " | ";
            var sb = new StringBuilder();
            sb.Append(diagnostic.Id);
            sb.Append(Separator);
            sb.Append(diagnostic.GetMessage());
            sb.Append(Separator);
            sb.Append("Line: ");
            sb.Append(diagnostic.Location.GetLineSpan().StartLinePosition.Line);

            return sb.ToString();
        }
    }
}

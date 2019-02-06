using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    public class SystemConsoleRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel model;
        private readonly MemberAccessExpressionSyntax dummyConsoleMemberAccessExpression;
        private readonly string[] memberNamesToReplace;

        public SystemConsoleRewriter(SemanticModel model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            string[] dummyConsoleSplitedFullName = typeof(DummyConsole.Console).FullName.Split('.');
            this.dummyConsoleMemberAccessExpression = (MemberAccessExpressionSyntax)this.GetTypeMemberAccessExpression(dummyConsoleSplitedFullName);
            this.memberNamesToReplace = new string[] { "WriteLine", "Write", "Read", "ReadKey", "ReadLine", "Out", "In", "Clear" };
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (!this.memberNamesToReplace.Any(mn => node.Name.Identifier.ValueText == mn))
            {
                return base.VisitMemberAccessExpression(node);
            }

            var symbolInfo = this.model.GetSymbolInfo(node.Name).Symbol;
            if (symbolInfo == null || !this.MustReplaceWithDummyConsole(symbolInfo))
            {
                return base.VisitMemberAccessExpression(node);
            }

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                this.dummyConsoleMemberAccessExpression,
                node.Name);
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (!this.memberNamesToReplace.Any(mn => node.Identifier.ValueText == mn))
            {
                return base.VisitIdentifierName(node);
            }

            var symbolInfo = this.model.GetSymbolInfo(node).Symbol;
            if (symbolInfo == null || !this.MustReplaceWithDummyConsole(symbolInfo))
            {
                return base.VisitIdentifierName(node);
            }

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                this.dummyConsoleMemberAccessExpression,
                node);
        }

        private bool MustReplaceWithDummyConsole(ISymbol symbolInfo)
        {
            if (symbolInfo.ContainingType.Name != "Console")
            {
                return false;
            }

            if (symbolInfo.ContainingType.ContainingNamespace.Name != "System")
            {
                return false;
            }

            if (!string.IsNullOrEmpty(symbolInfo.ContainingType.ContainingNamespace.ContainingNamespace.Name))
            {
                return false;
            }

            return true;
        }

        private ExpressionSyntax GetTypeMemberAccessExpression(string[] members)
        {
            if (members.Length == 1)
            {
                return SyntaxFactory.IdentifierName(members.Single());
            }

            var name = members[members.Length - 1];
            var remainingMembers = members.Take(members.Length - 1).ToArray();

            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                this.GetTypeMemberAccessExpression(remainingMembers),
                SyntaxFactory.IdentifierName(name));
        }
    }
}

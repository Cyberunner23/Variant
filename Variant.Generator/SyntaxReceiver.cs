using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Variant.Generator
{
    class SyntaxReceiver : ISyntaxReceiver
    {
        public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDecl && classDecl.AttributeLists.Count > 0)
            {
                CandidateClasses.Add(classDecl);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Variant.Generator
{
    [Generator]
    public class VariantGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource(VariantTemplate.GeneratedVariantAttributeName, VariantTemplate.GeneratedVariantAttributeText);

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) { return; }

            CSharpParseOptions options = (CSharpParseOptions)((CSharpCompilation)context.Compilation).SyntaxTrees[0].Options;
            SyntaxTree attributeSyntaxTree = CSharpSyntaxTree.ParseText(VariantTemplate.GeneratedVariantAttributeText, options);
            Compilation compilation = context.Compilation.AddSyntaxTrees(attributeSyntaxTree);

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            
            foreach (var candidateClass in receiver.CandidateClasses)
            {
                if (!HasGeneratedVariantAttribute(compilation, candidateClass)) { continue; }

                // Validate generic args

                var className = candidateClass.Identifier.ToString();
                //var classAccessModifier = candidateClass.Modifiers
                var classNamespace = GetClassNamespace(compilation, candidateClass);
                var classGenericArgumentNames = GetGenericArgumentNames(compilation, candidateClass);

                //VariantTemplate.GenerateVariantImplementation
  
            }
        }

        private bool HasGeneratedVariantAttribute(Compilation compilation, ClassDeclarationSyntax candidate)
        {
            SemanticModel model = compilation.GetSemanticModel(candidate.SyntaxTree);
            ITypeSymbol classSymbol = model.GetDeclaredSymbol(candidate);

            string metadataName = $"{VariantTemplate.GeneratedVariantAttributeNamespace}.{VariantTemplate.GeneratedVariantAttributeName}";
            INamedTypeSymbol attributeTypeSymbol = compilation.GetTypeByMetadataName(metadataName);
            bool hasAttribute = classSymbol.GetAttributes().Any(x => x.AttributeClass.Equals(attributeTypeSymbol));

            return hasAttribute;
        }

        private string GetClassNamespace(Compilation compilation, ClassDeclarationSyntax candidate)
        {
            var model = compilation.GetSemanticModel(candidate.SyntaxTree);
            var namespaceSyntax = candidate.SyntaxTree.GetRoot().DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().First();
            var namespaceSymbol = model.GetDeclaredSymbol(namespaceSyntax);

            return namespaceSymbol.Name;
        }

        private IEnumerable<string> GetGenericArgumentNames(Compilation compilation, ClassDeclarationSyntax candidate)
        {
            var model = compilation.GetSemanticModel(candidate.SyntaxTree);

            if (candidate.TypeParameterList == null) { return Enumerable.Empty<string>(); }

            var typeParameters = candidate.TypeParameterList.Parameters;
            var typeParameterNames = typeParameters.Select(x =>
            {
                var parameterSymbol = model.GetDeclaredSymbol(x);
                return parameterSymbol.Name;
            });

            return typeParameterNames;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FL.Variant.Generator
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
            //Debugger.Launch();

            context.AddSource(VariantTemplate.GeneratedVariantAttributeName, VariantTemplate.GeneratedVariantAttributeText);

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) { return; }

            CSharpParseOptions options = (CSharpParseOptions)((CSharpCompilation)context.Compilation).SyntaxTrees[0].Options;
            SyntaxTree attributeSyntaxTree = CSharpSyntaxTree.ParseText(VariantTemplate.GeneratedVariantAttributeText, options);
            Compilation compilation = context.Compilation.AddSyntaxTrees(attributeSyntaxTree);
            
            foreach (var candidateClass in receiver.CandidateClasses)
            {
                SemanticModel model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                ITypeSymbol classSymbol = model.GetDeclaredSymbol(candidateClass);

                if (!HasGeneratedVariantAttribute(compilation, classSymbol)) { continue; }

                // Validate generic args
                var genericArgumentNames = GetGenericArgumentNames(model, candidateClass);
                if (genericArgumentNames.Count() < 2)
                {
                    Location location = candidateClass.GetLocation();
                    Diagnostic diagnostic = Diagnostic.Create(Diagnostics.InvalidGenericArgumentCount, location);
                    context.ReportDiagnostic(diagnostic);
                    continue;
                }

                string @namespace = classSymbol.ContainingNamespace.Name;
                string name = candidateClass.Identifier.ToString();
                Accessibility accessibilityModifiers = classSymbol.DeclaredAccessibility;
                string accessibilityModifiersText = GetAccessibilityString(accessibilityModifiers);

                string generatedVariant = VariantTemplate.GenerateVariantImplementation(@namespace, accessibilityModifiersText, name, genericArgumentNames);
                context.AddSource($"{@namespace}_{name}_{string.Join("_", genericArgumentNames)}", generatedVariant);

                string generatedVariantConverter = VariantConverterTemplate.GenerateVariantConverterImplementation(@namespace, name, genericArgumentNames);
                context.AddSource($"{@namespace}_{name}_Converter_{string.Join("_", genericArgumentNames)}", generatedVariantConverter);
            }
        }

        private bool HasGeneratedVariantAttribute(Compilation compilation, ITypeSymbol classSymbol)
        {
            string metadataName = $"{VariantTemplate.GeneratedVariantAttributeNamespace}.{VariantTemplate.GeneratedVariantAttributeName}";
            INamedTypeSymbol attributeTypeSymbol = compilation.GetTypeByMetadataName(metadataName);
            bool hasAttribute = classSymbol.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeTypeSymbol));

            return hasAttribute;
        }

        private IEnumerable<string> GetGenericArgumentNames(SemanticModel model, ClassDeclarationSyntax candidate)
        {
            if (candidate.TypeParameterList == null) { return Enumerable.Empty<string>(); }

            var typeParameters = candidate.TypeParameterList.Parameters;
            var typeParameterNames = typeParameters.Select(x => { return model.GetDeclaredSymbol(x).Name; });

            return typeParameterNames;
        }

        private string GetAccessibilityString(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                    return "public";
                case Accessibility.Internal:
                    return "internal";
                default:
                    throw new InvalidOperationException($"Unhandled accessiblity: {accessibility}");
            }
        }
    }
}
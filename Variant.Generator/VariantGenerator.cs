using System.Text;

using Microsoft.CodeAnalysis;

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
            context.AddSource(VariantTemplateData.GeneratedVariantAttributeHintName, VariantTemplateData.GeneratedVariantAttributeText);

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) { return; }

            var builder = new StringBuilder();
        }
    }
}
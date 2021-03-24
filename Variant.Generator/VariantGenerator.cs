using System;
using System.Diagnostics;
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
            context.AddSource(VariantTemplate.GeneratedVariantAttributeHintName, VariantTemplate.GeneratedVariantAttributeText);

            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) { return; }

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            
            Debug.WriteLine("OUTPUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUT");
        }
    }
}
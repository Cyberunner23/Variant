using Microsoft.CodeAnalysis;

namespace FL.Variant.Generator
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor InvalidGenericArgumentCount = new DiagnosticDescriptor(
            "VRNT001",
            "Invalid Generic Argument Count",
            "A Variant must have at least two generic arguments",
            "VariantGenerator",
            DiagnosticSeverity.Error,
            true
        );
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FL.Variant.Generator
{
    internal sealed class VariantTemplate
    {
        private const string TResult        = "TResult";
        private const string FuncPrefix     = "func_";
        private const string ActionPrefix   = "action_";
        private const string SubclassPrefix = "Type_";

        public const string GeneratedVariantAttributeNamespace = "Variant";
        public const string GeneratedVariantAttributeName = "GeneratedVariantAttribute";
        public static readonly string GeneratedVariantAttributeText = $@"
            using System;
            namespace {GeneratedVariantAttributeNamespace}
            {{
                [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
                public class {GeneratedVariantAttributeName} : Attribute {{ }}
            }}";
        
        public static string GenerateVariantImplementation(string @namespace, string accessModifiers, string className, IEnumerable<string> genericArguments)
        {
            StringBuilder sb = new StringBuilder();

            string tab = "    ";
            string genericArgumentsText = string.Join(", ", genericArguments);
            string matchResultSignature = GenerateMatchResultSignature(genericArguments);
            string matchVoidSignature = GenerateMatchVoidSignature(genericArguments);

            string converterClassName = VariantConverterTemplate.GenerateVariantConverterClassName(className, genericArguments);

            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine($"{{");
            sb.AppendLine($"{tab}using System;");
            sb.AppendLine($"{tab}using System.Text.Json.Serialization;");
            sb.AppendLine();
            sb.AppendLine($"{tab}[JsonConverter(typeof({converterClassName}))]");
            sb.AppendLine($"{tab}{accessModifiers} abstract partial class {className}<{genericArgumentsText}>");
            sb.AppendLine($"{tab}{{");
            sb.AppendLine($"{tab}{tab}private {className}() {{ }}");
            sb.AppendLine($"{tab}{tab}public abstract {matchResultSignature};");
            sb.AppendLine($"{tab}{tab}public abstract {matchVoidSignature};");

            foreach (var genericArgument in genericArguments)
            {
                sb.AppendLine($"{tab}{tab}public static implicit operator {className}<{genericArgumentsText}>({genericArgument} value) => new {SubclassPrefix}{genericArgument}(value);");
            }
            
            sb.AppendLine();

            foreach (var genericArgument in genericArguments)
            {
                var subclassName = $"{SubclassPrefix}{genericArgument}";
                sb.AppendLine($"{tab}{tab}public sealed class {subclassName} : {className}<{genericArgumentsText}>");
                sb.AppendLine($"{tab}{tab}{{");
                sb.AppendLine($"{tab}{tab}{tab}private readonly {genericArgument} _value;");
                sb.AppendLine($"{tab}{tab}{tab}public {subclassName}({genericArgument} value) {{ _value = value; }}");
                sb.AppendLine($"{tab}{tab}{tab}public override {matchResultSignature} {{ return {FuncPrefix}{genericArgument}(_value); }}");
                sb.AppendLine($"{tab}{tab}{tab}public override {matchVoidSignature} {{ {ActionPrefix}{genericArgument}(_value); }}");
                sb.AppendLine($"{tab}{tab}}}");
                sb.AppendLine();
            }
            
            sb.AppendLine($"{tab}}}");
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        private static string GenerateMatchResultSignature(IEnumerable<string> genericArguments)
        {
            IEnumerable<string> arguments = genericArguments.Select(x => $"Func<{x}, {TResult}> {FuncPrefix}{x}");
            string argumentsText = string.Join(", ", arguments);
            return $"{TResult} Match<{TResult}>({argumentsText})";
        }
        
        private static string GenerateMatchVoidSignature(IEnumerable<string> genericArguments)
        {
            IEnumerable<string> arguments = genericArguments.Select(x => $"Action<{x}> {ActionPrefix}{x}");
            string argumentsText = string.Join(", ", arguments);
            return $"void Match({argumentsText})";
        }
    }
}

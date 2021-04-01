using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Variant.Generator
{
    internal class VariantConverterTemplate
    {
        private const string Tab = "    ";

        public static string GenerateVariantConverterClassName(string className, IEnumerable<string> genericArguments)
        {
            string genericArgumentsClassText = string.Join("_", genericArguments);
            return $"{className}Converter_{genericArgumentsClassText}";
        }

        public static string GenerateVariantConverterImplementation(string @namespace, string className, IEnumerable<string> genericArguments)
        {
            StringBuilder sb = new StringBuilder();
            
            string variantConverterClassName = GenerateVariantConverterClassName(className, genericArguments);
            string variantInnerConverterClassName = $"{variantConverterClassName}_Inner";
            string genericArgumentsText = string.Join(", ", genericArguments);
            string genericCommas = new string(',', genericArguments.Count() - 1);
            IEnumerable<string> genericArgumentsLower = genericArguments.Select(x => x.ToLower());

            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine($"{{");
            sb.AppendLine($"{Tab}using System;");
            sb.AppendLine($"{Tab}using System.Reflection;");
            sb.AppendLine($"{Tab}using System.Text.Json;");
            sb.AppendLine($"{Tab}using System.Text.Json.Serialization;");
            sb.AppendLine();
            sb.AppendLine($"{Tab}public class {variantConverterClassName} : JsonConverterFactory");
            sb.AppendLine($"{Tab}{{");
            sb.AppendLine($"{Tab}{Tab}public override bool CanConvert(Type typeToConvert)");
            sb.AppendLine($"{Tab}{Tab}{{");
            sb.AppendLine($"{Tab}{Tab}{Tab}return typeToConvert.GetGenericTypeDefinition() == typeof({className}<{genericCommas}>);");
            sb.AppendLine($"{Tab}{Tab}}}");
            sb.AppendLine();
            sb.AppendLine($"{Tab}{Tab}public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)");
            sb.AppendLine($"{Tab}{Tab}{{");

            int counter = 0;
            foreach (var genericArgument in genericArgumentsLower)
            {
                sb.AppendLine($"{Tab}{Tab}{Tab}Type {genericArgument} = typeToConvert.GetGenericArguments()[{counter}];");
                ++counter;
            }

            sb.AppendLine();

            sb.AppendLine($"{Tab}{Tab}{Tab}JsonConverter converter = (JsonConverter)Activator.CreateInstance(");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}typeof({variantInnerConverterClassName}<{genericCommas}>).MakeGenericType({string.Join(", ", genericArgumentsLower)}),");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}BindingFlags.Instance | BindingFlags.Public,");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}binder: null,");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}args: new object[] {{ }},");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}culture: null");
            sb.AppendLine($"{Tab}{Tab}{Tab});");
            sb.AppendLine();
            sb.AppendLine($"{Tab}{Tab}{Tab}return converter;");
            sb.AppendLine($"{Tab}{Tab}}}");
            sb.AppendLine();

            string subClass = GenerateConverterInternalClass(variantInnerConverterClassName, className, genericArguments);
            sb.AppendLine(subClass);
            sb.AppendLine($"{Tab}}}");
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        private static string GenerateConverterInternalClass(string innerClassName, string className, IEnumerable<string> genericArguements)
        {
            StringBuilder sb = new StringBuilder();

            string genericArgumentsText = string.Join(", ", genericArguements);

            sb.AppendLine($"{Tab}{Tab}private class {innerClassName}<{genericArgumentsText}> : JsonConverter<{className}<{genericArgumentsText}>>");
            sb.AppendLine($"{Tab}{Tab}{{");
            sb.AppendLine($"{Tab}{Tab}{Tab}public override void Write(Utf8JsonWriter writer, {className}<{genericArgumentsText}> value, JsonSerializerOptions options)");
            sb.AppendLine($"{Tab}{Tab}{Tab}{{");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}value.Match(");

            var matchArguments = genericArguements.Select(x => $"{Tab}{Tab}{Tab}{Tab}(val) => {{ JsonSerializer.Serialize(writer, val, options); }}");
            sb.Append(string.Join(",\n", matchArguments));
            sb.AppendLine($"\n{Tab}{Tab}{Tab}{Tab});");
            sb.AppendLine($"{Tab}{Tab}{Tab}}}");
            sb.AppendLine();
            sb.AppendLine($"{Tab}{Tab}{Tab}public override {className}<{genericArgumentsText}> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)");
            sb.AppendLine($"{Tab}{Tab}{Tab}{{");
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}Utf8JsonReader init = reader;");
            sb.AppendLine();

            var deserializeAttempts = genericArguements.Select(x => $"{Tab}{Tab}{Tab}{Tab}try {{ return JsonSerializer.Deserialize<{x}>(ref reader, options); }} catch {{ }}");
            sb.AppendLine(string.Join($"\n\n{Tab}{Tab}{Tab}{Tab}reader = init;\n\n", deserializeAttempts));
            sb.AppendLine();
            sb.AppendLine($"{Tab}{Tab}{Tab}{Tab}throw new JsonException();");
            sb.AppendLine($"{Tab}{Tab}{Tab}}}");
            sb.AppendLine($"{Tab}{Tab}}}");

            return sb.ToString();
        }
    }
}

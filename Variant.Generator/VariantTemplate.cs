using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Variant.Generator
{
    // TODO(AFL): Cleanup
    internal sealed class VariantTemplate
    {
        private string Namespace { get; }
        private string AccessModifier { get; }
        private string ClassName { get; }
        private IEnumerable<string> GenericArgumentNames { get; }

        public VariantTemplate(string @namespace, string accessModifier, string className, IEnumerable<string> genericArgumentNames)
        {
            Namespace = @namespace;
            AccessModifier = accessModifier;
            ClassName = className;
            GenericArgumentNames = genericArgumentNames;
        }

        public string GenerateVariantImplementation()
        {
            var sb = new StringBuilder(VariantTemplateData.GeneratedVariantText);

            var baseReplacementTable = new Dictionary<string, string>()
            {
                { VariantTemplateData.NamespaceTag, Namespace },
                { VariantTemplateData.AccessibilityModifierTag, AccessModifier },
                { VariantTemplateData.ClassNameTag, ClassName },
            };

            var classGenericArguments = GenerateClassGenericArguments();
            var matchResultMethodArguments = GenerateMatchResultMethodArguments();
            var matchVoidMethodArguments = GenerateMatchVoidMethodArguments();
            var implicitOperators = GenerateImplicitOperators();
            var replacementTable = new Dictionary<string, string>(baseReplacementTable)
            {
                { VariantTemplateData.ClassGenericArgumentsTag, classGenericArguments },
                { VariantTemplateData.MatchResultMethodArgumentsTag, matchResultMethodArguments},
                { VariantTemplateData.MatchVoidMethodArgumentsTag, matchVoidMethodArguments},
                { VariantTemplateData.ImplicitOperatorsTag, implicitOperators}
            };

            sb.ReplaceMultiple(replacementTable);

            return sb.ToString();
        }

        private string GenerateClassGenericArguments()
        {
            var genericArguments = string.Join(", ", GenericArgumentNames);
            return genericArguments;
        }

        private string GenerateMatchResultMethodArguments()
        {
            var methodArguments = GenericArgumentNames.Select(x => GenerateMatchResultMethodArgument(x));
            var methodArgumentsText = string.Join(", ", methodArguments);
            return methodArgumentsText;
        }

        private string GenerateMatchResultMethodArgument(string genericArgumentName)
        { 
            var argumentText = VariantTemplateData.MatchSingleResultArgumentText;
            var variablePrefix = VariantTemplateData.MatchSingleResultArgumentVariablePrefix;
            var classSingleGenericArgumentTag = VariantTemplateData.ClassSingleGenericArgumentTag;
            var matchSingleResultArgumentVariableTag = VariantTemplateData.MatchSingleResultArgumentVariableTag;

            var matchSingleResultArgumentVariable = $"{variablePrefix}{genericArgumentName}";
            argumentText = argumentText.Replace(classSingleGenericArgumentTag, genericArgumentName);
            argumentText = argumentText.Replace(matchSingleResultArgumentVariableTag, matchSingleResultArgumentVariable);

            return argumentText;
        }

        private string GenerateMatchVoidMethodArguments()
        {
            var methodArguments = GenericArgumentNames.Select(x => GenerateMatchVoidMethodArgument(x));
            var methodArgumentsText = string.Join(", ", methodArguments);
            return methodArgumentsText;
        }

        private string GenerateMatchVoidMethodArgument(string genericArgumentName)
        {
            var argumentText = VariantTemplateData.MatchSingleVoidArgumentText;
            var variablePrefix = VariantTemplateData.MatchSingleVoidArgumentVariablePrefix;
            var classSingleGenericArgumentTag = VariantTemplateData.ClassSingleGenericArgumentTag;
            var matchSingleVoidArgumentVariableTag = VariantTemplateData.MatchSingleVoidArgumentVariableTag;

            var matchSingleVoidArgumentVariable = $"{variablePrefix}{genericArgumentName}";
            argumentText = argumentText.Replace(classSingleGenericArgumentTag, genericArgumentName);
            argumentText = argumentText.Replace(matchSingleVoidArgumentVariableTag, matchSingleVoidArgumentVariable);

            return argumentText;
        }

        private string GenerateVariantSubclassName(string genericArgumentName)
        {
            var prefix = VariantTemplateData.VariantSubclassNamePrefix;
            return $"{prefix}{genericArgumentName}";
        }

        private string GenerateImplicitOperators()
        {
            var sb = new StringBuilder();

            var classGenericArguments = GenerateClassGenericArguments();
            var replacementTable = new Dictionary<string, string>()
            {
                { VariantTemplateData.ClassNameTag, ClassName },
                { VariantTemplateData.ClassGenericArgumentsTag, classGenericArguments },
            };

            var classSingleGenericArgumentTag = VariantTemplateData.ClassSingleGenericArgumentTag;
            var variantSubclassNameTag = VariantTemplateData.VariantSubclassNameTag;

            foreach (var genericArgumentName in GenericArgumentNames)
            {
                replacementTable[classSingleGenericArgumentTag] = genericArgumentName;
                replacementTable[variantSubclassNameTag] = GenerateVariantSubclassName(genericArgumentName);

                var implicitOperatorText = VariantTemplateData.ImplicitOperatorText;
                sb.AppendLine(implicitOperatorText);
                sb.ReplaceMultiple(replacementTable);
            }

            return sb.ToString();
        }
    }
}


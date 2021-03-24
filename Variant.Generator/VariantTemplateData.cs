namespace Variant.Generator
{
    class VariantTemplateData
    {
        #region GeneratedVariantAttribute

        public const string GeneratedVariantAttributeHintName = "GeneratedVariantAttribute";
        public const string GeneratedVariantAttributeText = @"
            using System;
            namespace Variant
            {
                [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
                public class GeneratedVariantAttribute : Attribute { }
            }";

        #endregion

        #region GeneratedVariant
        
        public static readonly string NamespaceTag                            = $"{{{nameof(NamespaceTag)}}}";
        public static readonly string AccessibilityModifierTag                = $"{{{nameof(AccessibilityModifierTag)}}}";
        public static readonly string ClassNameTag                            = $"{{{nameof(ClassNameTag)}}}";
        public static readonly string ClassGenericArgumentsTag                = $"{{{nameof(ClassGenericArgumentsTag)}}}";
        public static readonly string ClassSingleGenericArgumentTag           = $"{{{nameof(ClassSingleGenericArgumentTag)}}}";

        public static readonly string TResultTag                              = "TResult";
        public static readonly string SingleGenericArgumentTag                = "{SingleGenericArgumentTag}";
        public static readonly string VariantSubclassNamePrefix               = $"Type_";
        public static readonly string VariantSubclassNameTag                  = $"{{{nameof(VariantSubclassNameTag)}}}";

        public static readonly string MatchResultMethodArgumentsTag           = $"{{{nameof(MatchResultMethodArgumentsTag)}}}";
        public static readonly string MatchResultSignatureText                = $"public abstract {TResultTag} Match<{TResultTag}>({MatchResultMethodArgumentsTag})";
        public static readonly string MatchSingleResultArgumentVariableTag    = $"{{{nameof(MatchSingleResultArgumentVariableTag)}}}";
        public static readonly string MatchSingleResultArgumentVariablePrefix = $"func_";
        public static readonly string MatchSingleResultArgumentText           = $"Func<{ClassSingleGenericArgumentTag}, {TResultTag}> {MatchSingleResultArgumentVariableTag}";

        public static readonly string MatchVoidMethodArgumentsTag             = $"{{{nameof(MatchVoidMethodArgumentsTag)}}}";
        public static readonly string MatchVoidSignatureText                  = $"public abstract void Match({MatchVoidMethodArgumentsTag})";
        public static readonly string MatchSingleVoidArgumentVariableTag      = $"{{{nameof(MatchSingleVoidArgumentVariableTag)}}}";
        public static readonly string MatchSingleVoidArgumentVariablePrefix   = $"action_";
        public static readonly string MatchSingleVoidArgumentText             = $"Action<{ClassSingleGenericArgumentTag}> {MatchSingleVoidArgumentVariableTag}";

        public static readonly string ImplicitOperatorsTag = $"{{{nameof(ImplicitOperatorsTag)}}}";
        public static readonly string ImplicitOperatorText = $"public static implicit operator {ClassNameTag}<{ClassGenericArgumentsTag}>({ClassSingleGenericArgumentTag} value) => new {VariantSubclassNameTag}(value);";



        public static readonly string GeneratedVariantText = $@"
        namespace {NamespaceTag}
        {{
            {AccessibilityModifierTag} abstract partial class {ClassNameTag}<{ClassGenericArgumentsTag}>
            {{
                private {ClassNameTag}() {{ }}
                {MatchResultSignatureText};
                {MatchVoidSignatureText};
                {ImplicitOperatorsTag}
            }}
        }}";



        #endregion
    }
}

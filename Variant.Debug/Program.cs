using System;
using System.Collections.Generic;

using Variant.Generator;

namespace Variant.Debug
{
    class Program
    {
        string test = @"
        {{
            
        }}

";
        
        
        static void Main(string[] args)
        {
            var @namespace = "Variant";
            var accessModifier = "public";
            var className = "Variant";
            var genericArguments = new List<string>() { "T1", "T2", "T3" };
            
            var generated = VariantTemplate.GenerateVariantImplementation(@namespace, accessModifier, className, genericArguments);
            Console.WriteLine(generated);
        }
    }
}




/*

namespace SourceGeneratorSamples
{
    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        private const string attributeText = @"
using System;
namespace AutoNotify
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class AutoNotifyAttribute : Attribute
    {
        public AutoNotifyAttribute()
        {
        }
        public string PropertyName { get; set; }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // add the attribute text
            context.AddSource("AutoNotifyAttribute", attributeText);

            // retreive the populated receiver 
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            // we're going to create a new compilation that contains the attribute.
            // TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
            CSharpParseOptions options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
            Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(attributeText, Encoding.UTF8), options));

            // get the newly bound attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName("AutoNotify.AutoNotifyAttribute");
            INamedTypeSymbol notifySymbol = compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

            // loop over the candidate fields, and keep the ones that are actually annotated
            List<IFieldSymbol> fieldSymbols = new List<IFieldSymbol>();
            foreach (FieldDeclarationSyntax field in receiver.CandidateFields)
            {
                SemanticModel model = compilation.GetSemanticModel(field.SyntaxTree);
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    // Get the symbol being decleared by the field, and keep it if its annotated
                    IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                    if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                    {
                        fieldSymbols.Add(fieldSymbol);
                    }
                }
            }

            // group the fields by class, and generate the source
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in fieldSymbols.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context);
                context.AddSource($"{group.Key.Name}_autoNotify.cs", classSource);
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name} : {notifySymbol.ToDisplayString()}
    {{
");

            // if the class doesn't implement INotifyPropertyChanged already, add it
            if (!classSymbol.Interfaces.Contains(notifySymbol))
            {
                source.Append("public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            source.Append("} }");
            return source.ToString();
        }

        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the AutoNotify attribute from the field, and any associated data
            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.Append($@"
public {fieldType} {propertyName} 
{{
    get 
    {{
        return this.{fieldName};
    }}

    set
    {{
        this.{fieldName} = value;
        this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({propertyName})));
    }}
}}

");

            string chooseName(string fieldName, TypedConstant overridenNameOpt)
            {
                if (!overridenNameOpt.IsNull)
                {
                    return overridenNameOpt.Value.ToString();
                }

                fieldName = fieldName.TrimStart('_');
                if (fieldName.Length == 0)
                    return string.Empty;

                if (fieldName.Length == 1)
                    return fieldName.ToUpper();

                return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
            }

        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    CandidateFields.Add(fieldDeclarationSyntax);
                }
            }
        }
    }
}












namespace SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private const string EnumValidatorStub = @"
namespace EnumValidation
{ 
    internal static class EnumValidator
    {
        public static void Validate(System.Enum enumToValidate)
        {
            // This will be filled in by the generator once you call EnumValidator.Validate()
            // Trust me.
        }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // I should probably put something here
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = GenerateHelperClasses(context);

            var enumValidatorType = compilation.GetTypeByMetadataName("EnumValidation.EnumValidator")!;

            var infos = GetEnumValidationInfo(compilation, enumValidatorType);

            if (infos.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"namespace EnumValidation
{ 
    internal static class EnumValidator
    {");

                foreach (var info in infos)
                {
                    sb.AppendLine("        public static void Validate(" + info.EnumType.ToString() + " enumToValidate)");
                    sb.AppendLine("        {");

                    GenerateValidator(sb, info, "            ");

                    sb.AppendLine("        }");
                    sb.AppendLine();
                }

                sb.AppendLine(@"    }
}");

                context.AddSource("Validation.cs", sb.ToString());
            }
            else
            {
                context.AddSource("Validator.cs", EnumValidatorStub);
            }
        }

        private void GenerateValidator(StringBuilder sb, EnumValidationInfo info, string indent)
        {
            sb.AppendLine($"{indent}int intValue = (int)enumToValidate;");
            foreach ((int min, int max) in GetElementSets(info.Elements))
            {
                sb.AppendLine($"{indent}if (intValue >= {min} && intValue <= {max}) return;");
            }
            sb.AppendLine($"{indent}throw new System.ComponentModel.InvalidEnumArgumentException(\"{info.ArgumentName}\", intValue, typeof({info.EnumType}));");
        }

        private IEnumerable<(int min, int max)> GetElementSets(List<(string Name, int Value)> elements)
        {
            int min = 0;
            int? max = null;
            foreach (var info in elements)
            {
                if (max == null || info.Value != max + 1)
                {
                    if (max != null)
                    {
                        yield return (min, max.Value);
                    }
                    min = info.Value;
                    max = info.Value;
                }
                else
                {
                    max = info.Value;
                }
            }
            yield return (min, max.Value);
        }

        private static IEnumerable<EnumValidationInfo> GetEnumValidationInfo(Compilation compilation, INamedTypeSymbol enumValidatorType)
        {
            foreach (SyntaxTree? tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                foreach (var invocation in tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
                {
                    var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    if (SymbolEqualityComparer.Default.Equals(symbol.ContainingType, enumValidatorType))
                    {
                        // Note: This assumes the only method on enumValidatorType is the one we want.
                        // ie, I'm too lazy to check which invocation is being made :)
                        var argument = invocation.ArgumentList.Arguments.First().Expression;
                        var enumType = semanticModel.GetTypeInfo(argument).Type;
                        if (enumType == null)
                        {
                            continue;
                        }

                        var info = new EnumValidationInfo(enumType, argument.ToString());
                        foreach (var member in enumType.GetMembers())
                        {
                            if (member is IFieldSymbol
                                {
                                    IsStatic: true,
                                    IsConst: true,
                                    ConstantValue: int value
                                } field)
                            {
                                info.Elements.Add((field.Name, value));
                            }
                        }

                        info.Elements.Sort((e1, e2) => e1.Value.CompareTo(e2.Value));

                        yield return info;
                    }
                }
            }
        }

        private static Compilation GenerateHelperClasses(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            var tempCompilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(EnumValidatorStub, Encoding.UTF8), options));

            return tempCompilation;
        }

        private class EnumValidationInfo
        {
            public List<(string Name, int Value)> Elements = new();

            public ITypeSymbol EnumType { get; }
            public string ArgumentName { get; internal set; }

            public EnumValidationInfo(ITypeSymbol enumType, string argumentName)
            {
                this.EnumType = enumType;
                this.ArgumentName = argumentName;
            }
        }
    }
}
*/


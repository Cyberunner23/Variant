using System;
using System.Collections.Generic;

using Variant.Generator;

namespace Variant.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            var @namespace = "Variant";
            var accessModifier = "public";
            var className = "Variant";
            var genericArguments = new List<string>() { "T1", "T2", "T3" };

            var template = new VariantTemplate(@namespace, accessModifier, className, genericArguments);
            var generated = template.GenerateVariantImplementation();
            Console.WriteLine(generated);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Variant.Generator;

namespace Variant.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            string val = VariantConverterTemplate.GenerateVariantConverterImplementation("Variant", "Variant", new List<string> { "T1", "T2" });
            Console.WriteLine(val);
        }
    }
}

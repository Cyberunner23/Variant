using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Variant.Generator;

namespace Variant.Debug
{
    public class Container
    {
        public Variant<string, int> variant { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string val = VariantConverterTemplate.GenerateVariantConverterImplementation("Variant", "Variant", new List<string> { "T1", "T2" });
            //Console.WriteLine(val);

            var thing = new Container();
            thing.variant = 5;
            var res2 = JsonSerializer.Serialize(thing);
            var back = JsonSerializer.Deserialize<Container>(res2);

            back.variant.Match(
                (str) => Console.WriteLine($"STRING: {str}"),
                (i) =>   Console.WriteLine($"INT: {i}")
            );

        }
    }
}

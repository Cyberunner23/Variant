using System;

namespace Variant.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Variant<string, int, object, char, short> thing = 123;

            Console.WriteLine($"VARIANT: {thing.GetType()}");

            thing.Match(
                (string value) => { Console.WriteLine($"STRING: {value}"); },
                (int value) => { Console.WriteLine($"INT: {value}"); },
                (object value) => { Console.WriteLine($"OBJECT: {value}"); },
                (char value) => { Console.WriteLine($"CHAR: {value}"); },
                (short value) => { Console.WriteLine($"SHORT: {value}"); }
            );
        }
    }
}

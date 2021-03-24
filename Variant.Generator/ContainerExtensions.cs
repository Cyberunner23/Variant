using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Variant.Generator
{
    static class ContainerExtensions
    {
        public static void ReplaceMultiple(this StringBuilder sb, IDictionary<string, string> replacementTable)
        {
            foreach (var kvp in replacementTable)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> value, int count)
        {
            if (count < 0) { throw new ArgumentException("Count must be positive", nameof(count)); }
            return value.Take(value.Count() - count);
        }
    }
}

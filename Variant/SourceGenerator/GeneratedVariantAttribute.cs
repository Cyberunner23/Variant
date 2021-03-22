using System;

namespace Variant.SourceGenerator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GeneratedVariantAttribute : Attribute { }
}
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Variant
{
    [GeneratedVariant] [JsonConverter(typeof(VariantConverter_T1_T2))] public abstract partial class Variant<T1, T2> { }
    [GeneratedVariant] public abstract partial class Variant<T1, T2, T3> { }
    [GeneratedVariant] public abstract partial class Variant<T1, T2, T3, T4> { }
    [GeneratedVariant] public abstract partial class Variant<T1, T2, T3, T4, T5> { }
    [GeneratedVariant] public abstract partial class Variant<T1, T2, T3, T4, T5, T6> { }
    [GeneratedVariant] public abstract partial class Variant<T1, T2, T3, T4, T5, T6, T7> { }


    public class VariantConverter_T1_T2 : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetGenericTypeDefinition() == typeof(Variant<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type t1 = typeToConvert.GetGenericArguments()[0];
            Type t2 = typeToConvert.GetGenericArguments()[1];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(VariantConverter_T1_T2_Inner<,>).MakeGenericType(t1, t2),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null
            );

            return converter;
        }

        private class VariantConverter_T1_T2_Inner<T1, T2> : JsonConverter<Variant<T1, T2>>
        {
            public override void Write(Utf8JsonWriter writer, Variant<T1, T2> value, JsonSerializerOptions options)
            {
                value.Match(
                    (val) => { JsonSerializer.Serialize(val); },
                    (val) => { JsonSerializer.Serialize(val); }
                );
            }

            public override Variant<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Utf8JsonReader init = reader;

                try { return JsonSerializer.Deserialize<T1>(ref reader, options); } catch { }

                reader = init;

                try { return JsonSerializer.Deserialize<T2>(ref reader, options); } catch { }

                throw new JsonException();
            }
        }
    }

}

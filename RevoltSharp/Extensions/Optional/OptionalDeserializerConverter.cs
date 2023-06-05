using Newtonsoft.Json;
using Optionals;
using System;
using System.Linq;
using System.Reflection;

namespace RevoltSharp;

public class OptionalDeserializerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == null) throw new ArgumentNullException(nameof(objectType));
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        if (objectType == null) throw new ArgumentNullException(nameof(objectType));
        if (serializer == null) throw new ArgumentNullException(nameof(serializer));

        var innerType = objectType.GetGenericArguments()?.FirstOrDefault() ?? throw new InvalidOperationException("No inner type found.");
        var noneMethod = MakeStaticGenericMethodInfo(nameof(None), innerType);
        var someMethod = MakeStaticGenericMethodInfo(nameof(Some), innerType);

        if (reader.TokenType == JsonToken.Null)
        {
            return noneMethod.Invoke(null, Array.Empty<object>());
        }

        var innerValue = serializer.Deserialize(reader, innerType);

        if (innerValue == null)
        {
            return noneMethod.Invoke(null, Array.Empty<object>());
        }

        return someMethod.Invoke(noneMethod, new[] { innerValue });
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        if (serializer == null) throw new ArgumentNullException(nameof(serializer));

        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var innerType = value.GetType()?.GetGenericArguments()?.FirstOrDefault() ?? throw new InvalidOperationException("No inner type found.");
        var hasValueMethod = MakeStaticGenericMethodInfo(nameof(HasValue), innerType);
        var getValueMethod = MakeStaticGenericMethodInfo(nameof(GetValue), innerType);

        var hasValue = false;
        if (hasValueMethod != null)
        #pragma warning disable CS8605 // Unboxing a possibly null value.
            hasValue = (bool)hasValueMethod.Invoke(null, new[] { value });
        #pragma warning restore CS8605 // Unboxing a possibly null value.

        if (!hasValue)
        {
            writer.WriteNull();
            return;
        }

        var innerValue = getValueMethod.Invoke(null, new[] { value });
        serializer.Serialize(writer, innerValue);
    }

    private MethodInfo MakeStaticGenericMethodInfo(string name, params Type[] typeArguments)
    {
        return GetType()
            ?.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?.MakeGenericMethod(typeArguments)
            ?? throw new InvalidOperationException($"Could not make generic MethodInfo for method '{name}'.");
    }

    private static bool HasValue<T>(Optional<T> option) => option.HasValue;
    private static T? GetValue<T>(Optional<T> option) => option.HasValue ? option.Value : default(T);
    private static Optional<T> None<T>() => Optional.None<T>();
    private static Optional<T> Some<T>(T value) => Optional.Some(value);
}
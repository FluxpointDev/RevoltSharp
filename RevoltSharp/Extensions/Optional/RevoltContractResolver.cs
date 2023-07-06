using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RevoltSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Optionals;

public class RevoltContractResolver : DefaultContractResolver
{
    private static readonly TypeInfo _ienumerable = typeof(IEnumerable<ulong[]>).GetTypeInfo();
    private static readonly MethodInfo _shouldSerialize = typeof(RevoltContractResolver).GetTypeInfo().GetDeclaredMethod("ShouldSerialize");

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.Ignored)
            return property;

        if (member is PropertyInfo propInfo)
        {
            JsonConverter converter = GetConverter(property, propInfo, propInfo.PropertyType, 0);
            if (converter != null)
            {
                property.Converter = converter;
            }
        }
        else
            throw new InvalidOperationException($"{member.DeclaringType.FullName}.{member.Name} is not a property.");

        return property;
    }

    private static JsonConverter? GetConverter(JsonProperty property, PropertyInfo propInfo, Type type, int depth)
    {
        if (type.IsArray)
            return MakeGenericConverter(property, propInfo, typeof(ArrayConverter<>), type.GetElementType(), depth);

        if (type.IsConstructedGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            if (depth == 0 && genericType == typeof(Optional<>))
            {
                Type typeInput = propInfo.DeclaringType;
                Type innerTypeOutput = type.GenericTypeArguments[0];

                Type getter = typeof(Func<,>).MakeGenericType(typeInput, type);
                Delegate getterDelegate = propInfo.GetMethod.CreateDelegate(getter);
                MethodInfo shouldSerialize = _shouldSerialize.MakeGenericMethod(typeInput, innerTypeOutput);
                Func<object, Delegate, bool> shouldSerializeDelegate = (Func<object, Delegate, bool>)shouldSerialize.CreateDelegate(typeof(Func<object, Delegate, bool>));
                property.ShouldSerialize = x => shouldSerializeDelegate(x, getterDelegate);

                return MakeGenericConverter(property, propInfo, typeof(OptionalSerializerConverter<>), innerTypeOutput, depth);
            }
            else if (genericType == typeof(Nullable<>))
                return MakeGenericConverter(property, propInfo, typeof(NullableConverter<>), type.GenericTypeArguments[0], depth);
        }
        return null;
    }

    private static bool ShouldSerialize<TOwner, TValue>(object owner, Delegate getter)
    {
        return (getter as Func<TOwner, Optional<TValue>>)((TOwner)owner).HasValue;
    }

    private static JsonConverter? MakeGenericConverter(JsonProperty property, PropertyInfo propInfo, Type converterType, Type innerType, int depth)
    {
        TypeInfo genericType = converterType.MakeGenericType(innerType).GetTypeInfo();
        JsonConverter innerConverter = GetConverter(property, propInfo, innerType, depth + 1);
        return genericType.DeclaredConstructors.First().Invoke(new object[] { innerConverter }) as JsonConverter;
    }
}
internal class ArrayConverter<T> : JsonConverter
{
    private readonly JsonConverter _innerConverter;

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public ArrayConverter(JsonConverter innerConverter)
    {
        _innerConverter = innerConverter;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        List<T> result = new List<T>();
        if (reader.TokenType == JsonToken.StartArray)
        {
            reader.Read();
            while (reader.TokenType != JsonToken.EndArray)
            {
                T obj;
                if (_innerConverter != null)
                    obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                else
                    obj = serializer.Deserialize<T>(reader);
                result.Add(obj);
                reader.Read();
            }
        }
        return result.ToArray();
    }
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            writer.WriteStartArray();
            T[] a = (T[])value;
            for (int i = 0; i < a.Length; i++)
            {
                if (_innerConverter != null)
                    _innerConverter.WriteJson(writer, a[i], serializer);
                else
                    serializer.Serialize(writer, a[i], typeof(T));
            }

            writer.WriteEndArray();
        }
        else
            writer.WriteNull();
    }
}

internal class NullableConverter<T> : JsonConverter
    where T : struct
{
    private readonly JsonConverter _innerConverter;

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public NullableConverter(JsonConverter innerConverter)
    {
        _innerConverter = innerConverter;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        object value = reader.Value;
        if (value == null)
            return null;
        else
        {
            T obj;
            if (_innerConverter != null)
                obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer)!;
            else
                obj = serializer.Deserialize<T>(reader);
            return obj;
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
        {
            T? nullable = (T?)value;
            if (_innerConverter != null)
                _innerConverter.WriteJson(writer, nullable.Value, serializer);
            else
                serializer.Serialize(writer, nullable.Value, typeof(T));
        }
    }
}
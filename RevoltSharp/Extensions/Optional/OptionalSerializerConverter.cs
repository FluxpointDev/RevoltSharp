﻿using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp
{

    internal class OptionalSerializerConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public OptionalSerializerConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            T obj;
            if (_innerConverter != null)
            {
                object o = _innerConverter.ReadJson(reader, typeof(T), null, serializer);
                if (o is Optional<T>)
                    return o;

                obj = (T)o;
            }
            else
                obj = serializer.Deserialize<T>(reader);

            return Optional.Some(obj);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            value = ((Optional<T>)value!).Value;
            if (_innerConverter != null)
                _innerConverter.WriteJson(writer, value, serializer);
            else
                serializer.Serialize(writer, value, typeof(T));
        }
    }
}
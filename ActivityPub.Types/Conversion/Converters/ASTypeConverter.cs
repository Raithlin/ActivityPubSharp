// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json;
using System.Text.Json.Serialization;
using ActivityPub.Types.AS;
using InternalUtils;

namespace ActivityPub.Types.Conversion.Converters;

public class ASTypeConverter : JsonConverterFactory
{
    private readonly Type _asTypeType = typeof(ASType);


    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(_asTypeType);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Create an instance of the generic converter
        var constructedType = typeof(ASTypeConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(constructedType)!;
    }
}

internal class ASTypeConverter<T> : JsonConverter<T>
    where T : ASType
{
    private static readonly Func<TypeMap, T> TypeConstructor = TypeUtils.CreateDynamicConstructor<TypeMap, T>();

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeMap = JsonSerializer.Deserialize<TypeMap>(ref reader, options)
                      ?? throw new JsonException($"Can't convert {typeof(T)}: conversion to TypeMap returned null");

        return TypeConstructor(typeMap);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        // Simple pass-through of TypeMap
        => JsonSerializer.Serialize(writer, value.TypeMap, options);
}
using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace Jwt.Refresh.Token.Infra.Cosmos.Serializers;

public class SystemTextJsonCosmosSerializer(JsonSerializerOptions jsonSerializerOptions) : CosmosSerializer
{
    public override T FromStream<T>(Stream stream)
    {
        if (stream == null || stream.CanRead == false)
            return default!;

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions)!;
    }

    public override Stream ToStream<T>(T input)
    {
        var stream = new MemoryStream();
        Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = false });
        JsonSerializer.Serialize(writer, input, jsonSerializerOptions);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
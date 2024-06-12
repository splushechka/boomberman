namespace boooooom.JsonConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using boooooom.Entities.Enemies;

public class EnemyConverter : JsonConverter<Enemy>
{
    public override Enemy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var type = root.GetProperty("Type").GetString();

            Enemy? enemy = type switch
            {
                "ChaoticEnemy" => JsonSerializer.Deserialize<ChaoticEnemy>(root.GetRawText(), options),
                "LinearEnemy" => JsonSerializer.Deserialize<LinearEnemy>(root.GetRawText(), options),
                _ => throw new NotSupportedException($"Enemy type '{type}' is not supported.")
            };

            return enemy;
        }
    }

    public override void Write(Utf8JsonWriter writer, Enemy value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
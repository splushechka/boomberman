using System.Text.Json;
using System.Text.Json.Serialization;
using boooooom.Entities.Enemies;

namespace boooooom.JsonConverters;

public class EnemyListConverter : JsonConverter<List<Enemy>>
{
    public override List<Enemy> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<Enemy>();

        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var arrayEnumerator = root.EnumerateArray();

            while (arrayEnumerator.MoveNext())
            {
                var element = arrayEnumerator.Current;

                var type = element.GetProperty("Type").GetString();

                Enemy? enemy = type switch
                {
                    "ChaoticEnemy" => JsonSerializer.Deserialize<ChaoticEnemy>(element.GetRawText(), options),
                    "LinearEnemy" => JsonSerializer.Deserialize<LinearEnemy>(element.GetRawText(), options),
                    _ => throw new NotSupportedException($"Enemy type '{type}' is not supported.")
                };

                list.Add(enemy);
            }
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<Enemy> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var enemy in value)
        {
            JsonSerializer.Serialize(writer, enemy, options);
        }

        writer.WriteEndArray();
    }
}
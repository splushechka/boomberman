using System.Text.Json.Serialization;
using boooooom.Entities.Enemies;
using boooooom.JsonConverters;

namespace boooooom.Serializable;

public class LevelSettings
{
    public int PlayerX { get; set; }
    
    public int PlayerY { get; set; }
    
    public int Threshold { get; set; }
    
    public int Height { get; set; }
    
    public int Width { get; set; }
    
    [JsonConverter(typeof(EnemyListConverter))]
    public List<Enemy> Enemies { get; set; } = new ();
}
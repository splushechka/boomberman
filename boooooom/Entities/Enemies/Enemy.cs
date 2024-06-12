using System.Text.Json.Serialization;
using boooooom.Entities;
using boooooom.Enums;
using boooooom.Non_Entity_Classes;

namespace boooooom.Entities.Enemies;

public abstract class Enemy : ActiveEntity
{
    public Coordinates CurrentCoords { get; set; }

    [JsonConstructor]
    public Enemy(Coordinates currentCoords)
    {
        CurrentCoords = currentCoords;
        Lives = 1;
    }

    protected Enemy() { }

    public abstract string GetDrawSymbol();

    public abstract EnemyType GetEnemyType();

    public abstract Coordinates Move();
}
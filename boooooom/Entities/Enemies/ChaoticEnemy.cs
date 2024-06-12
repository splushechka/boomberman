using System.Text.Json.Serialization;
using boooooom.Entities;
using boooooom.Enums;
using boooooom.Non_Entity_Classes;

namespace boooooom.Entities.Enemies;

public class ChaoticEnemy : Enemy
{
    public override string GetDrawSymbol()
    {
        return "🧿";
    }

    [JsonConstructor]
    public ChaoticEnemy(Coordinates currentcoords) : base(currentcoords)
    {
        Lives = 1;
    }

    public override ActiveEntityType GetEntityType()
    {
        return ActiveEntityType.Enemy;
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Chaotic;
    }

    public override Coordinates Move()
    {
        Random rnd = new Random();
        int dx = rnd.Next(-1, 2);
        int dy = 0;

        if (dx == 0)
        {
            dy = rnd.Next(-1, 2);
        }

        return new Coordinates(dx, dy);
    }

    public override ActiveEntity Clone()
    {
        return new ChaoticEnemy(CurrentCoords) { Lives = Lives };
    }
}
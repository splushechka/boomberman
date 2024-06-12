using System.Text.Json.Serialization;
using boooooom.Entities;
using boooooom.Enums;
using boooooom.Non_Entity_Classes;

namespace boooooom.Entities.Enemies;

public class LinearEnemy : Enemy
{
    public bool IsHorizontal { get; set; }

    private int direction = 1;

    public override string GetDrawSymbol()
    {
        return "👾";
    }

    [JsonConstructor]
    public LinearEnemy(bool isHorizontal, Coordinates currentcoords) : base(currentcoords)
    {
        IsHorizontal = isHorizontal;
        Lives = 1;
    }

    public override ActiveEntityType GetEntityType()
    {
        return ActiveEntityType.Enemy;
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Linear;
    }

    public override Coordinates Move()
    {
        int nextX = 0;
        int nextY = 0;

        if (IsHorizontal)
        {
            nextX += direction;
        }
        else
        {
            nextY += direction;
        }

        return new Coordinates(nextX, nextY);
    }

    public void ReverseDirection()
    {
        direction *= -1;
    }

    public override ActiveEntity Clone()
    {
        return new LinearEnemy(IsHorizontal, CurrentCoords) { Lives = Lives };
    }
}
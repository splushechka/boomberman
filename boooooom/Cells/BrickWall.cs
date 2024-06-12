using boooooom.Entities;
using boooooom.Entities.Enemies;
using boooooom.Enums;

namespace boooooom.Cells;

public class BrickWall : Wall
{
    public bool IsDestroyed { get; private set; }

    public BrickWall() : base(CellType.BrickWall) { }

    public override string GetDrawSymbol()
    {
        bool hasPlayer = EntitiesOnCell.Any(e => e is PlayerEntity);
        bool hasBomb = BombOnCell != null;
        bool hasMultipleEnemies = EntitiesOnCell.Count(e => e is Enemy) > 1;

        if (hasMultipleEnemies)
        {
            return "⚔️"; // Символ для випадку, коли два вороги на одній клітинці
        }
        else if (IsDestroyed && hasPlayer && hasBomb)
        {
            return "🙀"; // Переляканий котик
        }
        else if (IsDestroyed && hasBomb)
        {
            return "💣"; // Символ для бомби на зруйнованій стіні
        }
        else if (IsDestroyed && hasPlayer && PrizeOnCell != null)
        {
            return "😻"; // Закоханий котик тільки якщо це гравець
        }
        else if (IsDestroyed && hasPlayer)
        {
            return "😸"; // Котик
        }
        else if (IsDestroyed && PrizeOnCell != null)
        {
            return PrizeOnCell.GetDrawSymbol();
        }
        else if (IsDestroyed && EntitiesOnCell.Any(e => e is Enemy)) // Перевірка наявності ворогів на клітинці
        {
            return (EntitiesOnCell.First(e => e is Enemy) as Enemy).GetDrawSymbol(); // Відображення символу ворога
        }
        else if (IsDestroyed && EntitiesOnCell.Count == 0)
        {
            return ".."; // Порожній символ, якщо приза немає
        }
        else
        {
            return "\U0001f9f1"; // Стіна
        }
    }

    public override bool HasPrize(out int prizeValue)
    {
        if (PrizeOnCell != null)
        {
            prizeValue = PrizeOnCell.Score;
            return true;
        }
        else
        {
            prizeValue = 0;
            return false;
        }
    }

    public override bool CanActiveEntityStep()
    {
        return IsDestroyed;
    }

    public override bool CanCellExplode()
    {
        return true;
    }

    public override void ExplodeCell(Cell[,] field)
    {
        base.ExplodeCell(field);
        IsDestroyed = true;
    }
}
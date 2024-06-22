using boooooom.Entities;
using boooooom.Entities.Enemies;
using boooooom.Enums;

namespace boooooom.Cells;

public class EmptyCell : Cell
{
    public bool Exploded { get; private set; }

    public EmptyCell(bool isPlayerOnCell = false) : base(CellType.Empty)
    {
        if (isPlayerOnCell)
        {
            EntitiesOnCell.Add(new PlayerEntity());
        }
        
        Exploded = false;
        BombOnCell = null;
    }

    public override bool CanActiveEntityStep()
    {
        return true;
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
    public override string GetDrawSymbol()
    {
        bool hasPlayer = EntitiesOnCell.Any(e => e is PlayerEntity);
        bool hasBomb = BombOnCell != null;
        bool hasPrize = PrizeOnCell != null;

        foreach (var entity in EntitiesOnCell)
        {
            if (entity is Enemy enemy)
            {
                return enemy.GetDrawSymbol(); // Викликаємо метод GetDrawSymbol ворога
            }
            else if (entity is PlayerEntity playerEntity)
            {
                return playerEntity.GetDrawSymbol(hasBomb, hasPrize, IsAffectedByExplosion); // Викликаємо метод GetDrawSymbol гравця
            }
        }

        if (BombOnCell != null)
        {
            return BombOnCell.GetDrawSymbol();
        }
        else if (IsAffectedByExplosion)
        {
            return "💥";
        }
        else if (PrizeOnCell != null)
        {
            return PrizeOnCell.GetDrawSymbol(); // Відображення символу приза
        }

        return "  ";
    }

    public override bool CanCellExplode()
    {
        return true;
    }

    public void PlaceBomb()
    {
        if (BombOnCell == null)
        {
            BombOnCell = new Bomb();
        }
    }
}
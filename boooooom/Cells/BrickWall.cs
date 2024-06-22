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
        if (IsDestroyed)
        {
            // Після руйнування стіни відображення буде керуватися класом EmptyCell
            return "..";
        }
        return "\U0001f9f1"; // Символ стіни
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
        ConvertToEmptyCell(field);
    }
    private void ConvertToEmptyCell(Cell[,] field)
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == this)
                {
                    var emptyCell = new EmptyCell
                    {
                        EntitiesOnCell = this.EntitiesOnCell,
                        BombOnCell = this.BombOnCell,
                        PrizeOnCell = this.PrizeOnCell,
                        IsAffectedByExplosion = this.IsAffectedByExplosion
                    };

                    field[y, x] = emptyCell;
                    return;
                }
            }
        }
    }
}

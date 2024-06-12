using boooooom.Entities;
using boooooom.Enums;

namespace boooooom.Cells;

public abstract class Cell
{
    public CellType CellType { get; init; }

    public List<ActiveEntity> EntitiesOnCell { get; set; }
    
    public Bomb? BombOnCell { get; set; }

    public Prize? PrizeOnCell { get; set; }

    public bool IsAffectedByExplosion { get; set; }

    public Cell(CellType cellType)
    {
        CellType = cellType;
        EntitiesOnCell = new List<ActiveEntity>();
    }

    public abstract bool HasPrize(out int prizeValue);
    public abstract bool CanActiveEntityStep();
    public abstract bool CanCellExplode();

    public virtual void ExplodeCell(Cell[,] field)
    {
        IsAffectedByExplosion = true;

        var entitiesToRemove = new List<ActiveEntity>();
        foreach (var entity in EntitiesOnCell)
        {
            entity.MinusLiveEntity();
            if (entity.IsDead() && entity.GetEntityType() == ActiveEntityType.Enemy)
            {
                // Add a heart as a prize when an enemy dies
                PrizeOnCell = new Heart();
                entitiesToRemove.Add(entity);
            }
        }

        foreach (var entity in entitiesToRemove)
        {
            EntitiesOnCell.Remove(entity);
        }
    }

    public abstract string GetDrawSymbol();
}
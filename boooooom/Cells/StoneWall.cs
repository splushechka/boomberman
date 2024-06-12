using boooooom.Enums;

namespace boooooom.Cells;

public class StoneWall : Wall
{
    public StoneWall() : base(CellType.StoneWall) { }

    public override bool CanActiveEntityStep()
    {
        return false;
    }
    public override string GetDrawSymbol()
    {
        return "🌫️";
    }

    public override bool CanCellExplode()
    {
        return false;
    }
    public override bool HasPrize(out int prizeValue)
    {
        prizeValue = 0;
        return false;
    }
}

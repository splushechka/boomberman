namespace boooooom.Entities;

public class Heart : Prize
{
    public Heart() : base(1) { } 

    public override string GetDrawSymbol() 
    {
        return "❤️"; 
    }
}
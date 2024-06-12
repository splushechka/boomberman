namespace boooooom.Non_Entity_Classes;

public class Coordinates
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinates Add(Coordinates other)
    {
        return new Coordinates(X + other.X, Y + other.Y);
    }
    public bool Equals(Coordinates other)
    {
        return X == other.X && Y == other.Y;
    }
    public Coordinates Clone()
    {
        return new Coordinates(X, Y);
    }
    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"{X}:{Y}";
    }
}

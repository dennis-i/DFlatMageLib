namespace DFlatMage.Common;

public readonly record struct Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; init; }
    public int Y { get; init; }
}

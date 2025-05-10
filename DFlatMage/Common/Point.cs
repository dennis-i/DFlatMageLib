namespace DFlatMage.Common;

public readonly record struct Point(int X, int Y)
{
    public bool Inside(Rect r)
    {
        return X.InRange(r.LeftTop.X,r.RightBottom.X) && Y.InRange(r.LeftTop.Y,r.RightBottom.Y);
    }
}

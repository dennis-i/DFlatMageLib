namespace DFlatMage.Common;

public readonly record struct Point(int X, int Y)
{
    
    
    public static implicit operator Point(ValueTuple<int,int> tuple) => new(tuple.Item1,tuple.Item2);
    public bool Inside(Rect r)
    {
        return X.InRange(r.LeftTop.X,r.RightBottom.X) && Y.InRange(r.LeftTop.Y,r.RightBottom.Y);
    }
}

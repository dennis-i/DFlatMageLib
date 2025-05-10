using System.Numerics;

namespace DFlatMage.Common;

public readonly record struct Rect
{
    public Rect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public static Rect Zero => new Rect(0, 0, 0, 0);


    public Point LeftTop => new(X, Y);
    public Point RightBottom => new(X + Width, Y + Height);

    public Rect OverlappedRect(Rect other)
    {
        if (LeftTop.Inside(other))
        {
            int right = Math.Min(RightBottom.X, other.RightBottom.X);
            int bottom = Math.Min(RightBottom.Y, other.RightBottom.Y);
            return new Rect(X, Y, right - X, bottom - Y);
        }

        if (RightBottom.Inside(other))
        {
            int left = Math.Max(LeftTop.X, other.LeftTop.X);
            int top = Math.Max(LeftTop.Y, other.LeftTop.Y);
            return new Rect(left, top, Width - left, Height - top);
        }

        if (other.LeftTop.Inside(this))
        {
            int right = Math.Min(RightBottom.X, other.RightBottom.X);
            int bottom = Math.Min(RightBottom.Y, other.RightBottom.Y);
            return new Rect(other.LeftTop.X, other.LeftTop.Y, right - other.LeftTop.X, bottom - other.LeftTop.Y);
        }

        if (other.RightBottom.Inside(this))
        {
            int left = Math.Max(LeftTop.X, other.LeftTop.X);
            int top = Math.Max(LeftTop.Y, other.LeftTop.Y);
            return new Rect(left, top, other.Width - left, other.Height - top);
        }
        

        return Rect.Zero;
    }
}

public static class Extensions
{
    public static bool InRange(this int n, int min, int max)
    {
        return n >= min && n <= max;
    }
}
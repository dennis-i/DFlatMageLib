using DFlatMage.Interfaces;

using System.Runtime.CompilerServices;

namespace DFlatMage.Impl.ImageImpl;

internal partial class ImageImpl
{
    public override string ToString()
    {
        return $"Image:{NumPlanes} planes,{Height} rows,{Width} cols,{Bpp} bpp";
    }


    private bool Equals(ImageImpl other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return NumPlanes == other.NumPlanes &&
               Width == other.Width &&
               Height == other.Height &&
               Bpp == other.Bpp &&
               ContentEqual(other._planes);
    }

    private bool ContentEqual(IPlaneData[]? other)
    {
        if (other == null) return false;
        if (_planes.Length != other.Length) return false;

        for (int i = 0; i < other.Length; ++i)
        {
            var d1 = _planes[i].GetData();
            var d2 = other[i].GetData();

            if (d1.Length != d2.Length)
                return false;

            for (int j = 0; j < d1.Length; ++j)
            {
                if (d1[j] != d2[j])
                    return false;
            }

            if (!d1.SequenceEqual(d2))
                return false;
        }
        return true;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((obj as ImageImpl)!);
    }

    public override int GetHashCode() => HashCode.Combine(NumPlanes, Width, Height, Bpp);


    private static bool InRange(int val, int range) => (uint)val < (uint)range;
    private static bool InRange(double val, double range) => (uint)val < (uint)range;
    private void ThrowIfNotInRange(int val, int range, [CallerArgumentExpression("val")] string expression = "")
    {
        if (!InRange(val, range))
            throw new OutOfImageRangeException($"parameter {expression}:{val} out of image range.{this}");
    }


    private int NormGetPixel(int plane, double normRow, double normCol)
    {
        if (!InRange(normRow, 1.0) || !InRange(normCol, 1.0))
            throw new ArgumentException("Normalized values limited to 0.0 - 1.0 range");

        var y = normRow * (_nRows - 1);

        var x = normCol * (_nRows - 1);

        return BilinearInterpolate(plane, y, x);
    }


    private int BilinearInterpolate(int plane, double x, double y)
    {
        // Get integer pixel coordinates
        int x0 = (int)Math.Floor(x);
        int x1 = Math.Min(x0 + 1, _nCols - 1); // Ensure x1 doesn't go out of bounds
        int y0 = (int)Math.Floor(y);
        int y1 = Math.Min(y0 + 1, _nRows - 1); // Ensure y1 doesn't go out of bounds

        // Get the four surrounding pixel colors
        int Q11 = GetPixUnsafe(plane, x0, y0); // Top-left
        int Q21 = GetPixUnsafe(plane, x1, y0); // Top-right
        int Q12 = GetPixUnsafe(plane, x0, y1); // Bottom-left
        int Q22 = GetPixUnsafe(plane, x1, y1); // Bottom-right

        // Linear interpolation in the x direction
        double v1 = Lerp(Q11, Q21, x - x0);
        double v2 = Lerp(Q12, Q22, x - x0);


        // Linear interpolation in the y direction
        int val = (int)Lerp(v1, v2, y - y0);

        return val;
    }

    // Linear interpolation function
    public static double Lerp(double start, double end, double t) => start + t * (end - start);

}



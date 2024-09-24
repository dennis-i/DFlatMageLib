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
    private void ThrowIfNotInRange(int val, int range, [CallerArgumentExpression("val")] string expression = "")
    {
        if (!InRange(val, range))
            throw new OutOfImageRangeException($"parameter {expression}:{val} out of image range.{this}");
    }
}



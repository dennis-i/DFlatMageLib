using DFlatMage.Impl.ImagePlanes;

namespace DFlatMage.Interfaces;

public interface IPlaneData : IDisposable
{
    static IPlaneData Create(int nRows, int nCols, int bpp) => bpp switch
    {
        8 => new PlaneData8bit(nRows, nCols),
        _ => new NonSupportedPlaneData()
    };
    Span<byte> GetData();

    int GetPix(int row, int col);
    Span<byte> GetRow(int row);
    void SetPix(int row, int col, int val);
}

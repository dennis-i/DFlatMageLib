using DFlatMage.Impl;

namespace DFlatMage.Interfaces;

public interface IPlaneData : IDisposable
{
    static IPlaneData Create(int nRows, int nCols, int bpp) => bpp switch
    {
        8 => new PlaneData8bit(nRows, nCols),
        _ => new NonSupportedPlaneData()
    };


    int GetPix(int row, int col);
    void SetPix(int row, int col, int val);
}

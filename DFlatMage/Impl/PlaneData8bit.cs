using DFlatMage.Interfaces;

namespace DFlatMage.Impl;

internal class PlaneData8bit : IPlaneData
{
    public PlaneData8bit(int nRows, int nCols)
    {
        _data = new byte[nRows * nCols];
    }
    private byte[] _data;

    public void Dispose()
    {
        _data = [];
    }
}

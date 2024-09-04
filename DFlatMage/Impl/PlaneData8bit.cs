using DFlatMage.Interfaces;

namespace DFlatMage.Impl;



internal class PlaneData8bit : IPlaneData
{
    public PlaneData8bit(int nRows, int nCols)
    {
        _stride = nCols;
        _data = new byte[nRows * _stride];
    }
    private byte[] _data;
    private int _stride;
    public void Dispose()
    {
        _data = [];
    }

    public int GetPix(int row, int col) => _data[_stride * row + col];


    public void SetPix(int row, int col, int val) => _data[_stride * row + col] = (byte)val;

}

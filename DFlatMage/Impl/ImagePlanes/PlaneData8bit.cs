using System.Runtime.CompilerServices;
using DFlatMage.Interfaces;

namespace DFlatMage.Impl.ImagePlanes;



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
        _stride = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetPix(int row, int col) => _data[_stride * row + col];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPix(int row, int col, int val) => _data[_stride * row + col] = (byte)val;

    public Span<byte> GetRow(int row) => _data.AsSpan(_stride * row, _stride);

    public Span<byte> GetData() => _data;

}

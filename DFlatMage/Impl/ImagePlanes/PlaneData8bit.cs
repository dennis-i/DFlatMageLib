using System.Runtime.CompilerServices;
using DFlatMage.Interfaces;

namespace DFlatMage.Impl.ImagePlanes;



internal class PlaneData8bit : IPlaneData
{
    public PlaneData8bit(int nRows, int nCols)
    {
        _stride = nCols;
        _data = new byte[nRows * _stride];
        _rowBase = new int[nRows];
        for (int i = 0; i < nRows; ++i)
            _rowBase[i] = i * _stride;
    }
    private byte[] _data;
    private int _stride;
    private readonly int[] _rowBase;
    public void Dispose()
    {
        _data = [];
        _stride = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetPix(int row, int col) => _data[_rowBase[row] + col];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPix(int row, int col, int val) => _data[_rowBase[row] + col] = (byte)val;

    public Span<byte> GetRow(int row) => _data.AsSpan(_rowBase[row], _stride);

    public Span<byte> GetData() => _data;

}

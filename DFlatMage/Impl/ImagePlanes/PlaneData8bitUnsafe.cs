using DFlatMage.Interfaces;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DFlatMage.Impl.ImagePlanes;

internal unsafe class PlaneData8bitUnsafe : IPlaneData
{
    public PlaneData8bitUnsafe(int nRows, int nCols)
    {
        _stride = nCols;
        _dataSize = nRows * _stride;
        _data = Marshal.AllocHGlobal(_dataSize);
        Unsafe.InitBlockUnaligned(_data.ToPointer(), 0, (uint)_dataSize);
    }
    private IntPtr _data;
    private int _stride;
    private int _dataSize;
    public void Dispose()
    {
        Marshal.FreeHGlobal(_data);
        _data = IntPtr.Zero;
        _dataSize = 0;
    }

    public int GetPix(int row, int col)
    {
        byte* ptr = (byte*)IntPtr.Add(_data, _stride * row + col).ToPointer();
        return *ptr;
    }


    public void SetPix(int row, int col, int val)
    {
        byte* ptr = (byte*)IntPtr.Add(_data, _stride * row + col).ToPointer();
        *ptr = (byte)val;
    }

    public Span<byte> GetRow(int row) => new Span<byte>(IntPtr.Add(_data, _stride * row).ToPointer(), _stride);

    public Span<byte> GetData() => new Span<byte>(_data.ToPointer(), _dataSize);
}

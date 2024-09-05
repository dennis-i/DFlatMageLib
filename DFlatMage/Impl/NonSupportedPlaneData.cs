using DFlatMage.Interfaces;

namespace DFlatMage.Impl;

internal class NonSupportedPlaneData : IPlaneData
{
    public void Dispose()
    {
        
    }

    public Span<byte> GetData()
    {
        throw new NotImplementedException();
    }

    public int GetPix(int row, int col)
    {
        throw new NotImplementedException();
    }

    public Span<byte> GetRow(int row)
    {
        throw new NotImplementedException();
    }

    public void SetPix(int row, int col, int val)
    {
        throw new NotImplementedException();
    }
}

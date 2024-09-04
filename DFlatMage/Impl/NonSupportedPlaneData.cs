using DFlatMage.Interfaces;

namespace DFlatMage.Impl;

internal class NonSupportedPlaneData : IPlaneData
{
    public void Dispose()
    {
        
    }

    public int GetPix(int row, int col)
    {
        throw new NotImplementedException();
    }

    public void SetPix(int row, int col, int val)
    {
        throw new NotImplementedException();
    }
}

using DFlatMage.Interfaces;

namespace DFlatMage.Impl;

internal class Image8bit : IImage
{
    public Image8bit(int nPlanes, int nRows, int nCols)
    {
        _planes = new IPlaneData[nPlanes];
        for (int i = 0; i < nPlanes; i++)
        {
            _planes[i] = IPlaneData.Create(nRows, nCols, 8);
        }
    }

    private readonly IPlaneData[] _planes;

    public int NumPlanes => _planes.Length;

    public void Dispose()
    {
        foreach (IPlaneData plane in _planes)
            plane.Dispose();
    }

}

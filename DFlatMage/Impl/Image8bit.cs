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
        _nRows = nRows;
        _nCols = nCols;
    }
    private readonly int _nRows;
    private readonly int _nCols;
    private readonly IPlaneData[] _planes;

    public int NumPlanes => _planes.Length;

    public int Height => _nRows;

    public int Width => _nCols;

    public void Dispose()
    {
        foreach (IPlaneData plane in _planes)
            plane.Dispose();
    }

}

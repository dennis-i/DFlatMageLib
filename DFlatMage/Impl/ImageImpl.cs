using DFlatMage.Interfaces;
using System.Numerics;

namespace DFlatMage.Impl;


internal class ImageImpl : IImage
{
    public ImageImpl(int nPlanes, int nRows, int nCols, int bpp)
    {
        _planes = new IPlaneData[nPlanes];
        for (int i = 0; i < nPlanes; i++)
        {
            _planes[i] = IPlaneData.Create(nRows, nCols, bpp);
        }
        _nRows = nRows;
        _nCols = nCols;
    }

    private int _nRows;
    private int _nCols;

    private IPlaneData[] _planes;

    public int NumPlanes => _planes.Length;

    public int Height => _nRows;

    public int Width => _nCols;

    public void Dispose()
    {
        foreach (var pl in _planes)
            pl.Dispose();
        _planes = [];
        _nRows = 0;
        _nCols = 0;
    }

    public int GetPix(int plane, int row, int col) => _planes[plane].GetPix(row, col);


    public void SetPix(int plane, int row, int col, int val) => _planes[plane].SetPix(row, col, val);

    public void Save(string filePath, ImageFormatType type)
    {
        var writer = IImageWriter.GetWriter(type);
        writer.Write(filePath, this);

    }

    public Span<byte> GetRow(int plane, int row) => _planes[plane].GetRow(row);

    public Span<byte> GetPlane(int plane)=> _planes[plane].GetData();
   
}



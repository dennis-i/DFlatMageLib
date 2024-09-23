using DFlatMage.Enums;
using DFlatMage.Interfaces;


namespace DFlatMage.Impl.ImageImpl;
internal partial class ImageImpl : IImage
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
        _bpp = bpp;
    }

    private int _nRows;
    private int _nCols;
    private int _bpp;

    private IPlaneData[] _planes;

    public int NumPlanes => _planes.Length;

    public int Height => _nRows;

    public int Width => _nCols;

    public int Bpp => _bpp;

    public void Dispose()
    {
        foreach (var pl in _planes)
            pl.Dispose();
        _planes = [];
        _nRows = 0;
        _nCols = 0;
        _bpp = 0;
    }

    public int GetPixUnsafe(int plane, int row, int col) => _planes[plane].GetPix(row, col);


    public void SetPixUnsafe(int plane, int row, int col, int val) => _planes[plane].SetPix(row, col, val);


    public int GetPix(int plane, int row, int col)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(row, _nRows);
        ThrowIfNotInRange(col, _nCols);
        return _planes[plane].GetPix(row, col);
    }


    public void SetPix(int plane, int row, int col, int val)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(row, _nRows);
        ThrowIfNotInRange(col, _nCols);
        _planes[plane].SetPix(row, col, val);
    }

    public void Save(string filePath, ImageFormatType type)
    {
        var writer = IImageWriter.GetWriter(type);
        writer.Write(filePath, this);
    }

    public Span<byte> GetRow(int plane, int row)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(row, _nRows);
        return _planes[plane].GetRow(row);
    }

    public Span<byte> GetPlane(int plane)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        return _planes[plane].GetData();
    }

   
}



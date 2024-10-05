using DFlatMage.Common;
using DFlatMage.Enums;
using DFlatMage.Interfaces;
using System.Numerics;


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

    public IImage Crop(Rect rect)
    {
        var result = new ImageImpl(NumPlanes, rect.Height, rect.Width, _bpp);
        for (int p = 0; p < NumPlanes; p++)
        {
            for (int y = 0; y < rect.Height; ++y)
            {
                var srcRow = GetRow(p, y + rect.Y);
                var dstRow = result.GetRow(p, y);
                srcRow.Slice(rect.X, rect.Width).CopyTo(dstRow);
            }
        }
        return result;
    }

    public IImage Scale(double xFactor, double yFactor)
    {
        int newWidth = (int)Math.Round(xFactor * _nCols);
        int newHeight = (int)Math.Round(yFactor * _nRows);

        var result = new ImageImpl(NumPlanes, newHeight, newWidth, _bpp);

        for (int p = 0; p < NumPlanes; ++p)
        {
            for (int y = 0; y < newHeight; ++y)
            {
                int srcY = (int)Math.Floor(y / yFactor);

                for (int x = 0; x < newWidth; ++x)
                {
                    int srcX = (int)Math.Floor(x / xFactor);
                    var pix = GetPix(p, srcY, srcX);
                    result.SetPix(p, y, x, pix);
                }
            }
        }
        return result;
    }

   
}



using DFlatMage.Enums;
using DFlatMage.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DFlatMage.Impl;

public class OutOfImageRangeException : Exception
{
    public OutOfImageRangeException(string text) : base(text) { }
}

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

    public int GetPix(int plane, int row, int col) => _planes[plane].GetPix(row, col);


    public void SetPix(int plane, int row, int col, int val) => _planes[plane].SetPix(row, col, val);

    public void Save(string filePath, ImageFormatType type)
    {
        var writer = IImageWriter.GetWriter(type);
        writer.Write(filePath, this);

    }

    public Span<byte> GetRow(int plane, int row) => _planes[plane].GetRow(row);

    public Span<byte> GetPlane(int plane) => _planes[plane].GetData();

    public void DrawLine(int plane, int row1, int col1, int row2, int col2, int val)
    {
        ThrowIfNotInRange(row1, _nRows);
        ThrowIfNotInRange(row2, _nRows);
        ThrowIfNotInRange(col1, _nCols);
        ThrowIfNotInRange(col2, _nCols);

        int xSize = col2 - col1;
        int ySize = row2 - row1;
        int cnt = 0;
        if (xSize > ySize)
        {

            double factor = (double)ySize / (double)xSize;
            for (int x = col1; x < col2; x++)
            {
                int y = (int)Math.Round(row1 + cnt * factor);
                SetPix(plane, y, x, val);
                cnt++;
            }
        }
        else
        {
            double factor = (double)xSize / (double)ySize;
            for (int y = row1; y < row2; y++)
            {
                int x = (int)Math.Round(col1 + cnt * factor);
                SetPix(plane, y, x, val);
                cnt++;
            }
        }
    }


    public override string ToString()
    {
        return $"Image:{NumPlanes} planes,{Height} rows,{Width} cols,{Bpp} bpp";
    }

    private static bool InRange(int val, int range) => (uint)val < (uint)range;
    private void ThrowIfNotInRange(int val, int range, [CallerArgumentExpression("val")] string expression = "")
    {
        if (!InRange(val, range))
            throw new OutOfImageRangeException($"parameter {expression}:{val} out of image range.{this}");
    }
}



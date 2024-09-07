using DFlatMage.Enums;
using DFlatMage.Impl;

namespace DFlatMage.Interfaces;

public interface IImage : IDisposable
{
    static IImage Create(int nPlanes, int nRows, int nCols, Bpp bpp) => new ImageImpl(nPlanes, nRows, nCols, (int)bpp);

    int NumPlanes { get; }
    int Height { get; }
    int Width { get; }
    int Bpp { get; }

    int GetPix(int plane, int row, int col);
    void SetPix(int plane, int row, int col, int val);
    void Save(string filePath, ImageFormatType type);
    Span<byte> GetRow(int plane, int row);
    Span<byte> GetPlane(int plane);
    void DrawLine(int plane, int row1, int col1, int row2, int col2, int val);
}

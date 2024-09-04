using DFlatMage.Impl;

namespace DFlatMage.Interfaces;






public interface IImage : IDisposable
{
    static IImage Create(int nPlanes, int nRows, int nCols, int bpp) => new ImageImpl(nPlanes, nRows, nCols, bpp);

    int NumPlanes { get; }
    int Height { get; }
    int Width { get; }



    int GetPix(int plane, int row, int col);
    void SetPix(int plane, int row, int col, int val);
    void Save(string filePath, ImageFormatType type);
}

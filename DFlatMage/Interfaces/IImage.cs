using DFlatMage.Common;
using DFlatMage.Enums;
using DFlatMage.Impl.ImageImpl;

namespace DFlatMage.Interfaces;

public interface IImage : IDisposable
{
    static IImage Create(int nPlanes, int nRows, int nCols, Bpp bpp) => new ImageImpl(nPlanes, nRows, nCols, (int)bpp);

    int NumPlanes { get; }
    int Height { get; }
    int Width { get; }
    int Bpp { get; }

    int GetPixUnsafe(int plane, int row, int col);
    int GetPix(int plane, int row, int col);
    void SetPixUnsafe(int plane, int row, int col, int val);
    void SetPix(int plane, int row, int col, int val);
    void Save(string filePath, ImageFormatType type);
    Span<byte> GetRow(int plane, int row);
    Span<byte> GetPlane(int plane);
    void DrawLine(int plane, int row1, int col1, int row2, int col2, int val);
    void DrawLine(int plane, Point p1, Point p2, int val) => DrawLine(plane, p1.Y, p1.X, p2.Y, p2.X, val);
    void DrawRect(int plane, Rect rect, int val);
    void DrawRect(Range planes, Rect rect, int val);
    void DrawCircle(int plane, Point center, int radius, int val);
    void DrawPath(int plane, IReadOnlyList<Point> path, int val);
    IImage Crop(Rect rect);
    IImage Scale(double xFactor, double yFactor);
}

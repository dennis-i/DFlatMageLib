using DFlatMage.Impl;

namespace DFlatMage.Interfaces;
public interface IImage : IDisposable
{
    int NumPlanes { get; }

    static IImage Create(int nPlanes, int nRows, int nCols, int bpp) => bpp switch
    {
        8 => new Image8bit(nPlanes, nRows, nCols),
        _ => new NotSupportedImage()
    };
}

using DFlatMage.Interfaces;
using System.Numerics;

namespace DFlatMage.Impl;
internal class NotSupportedImage : IImage
{
    public int NumPlanes => 0;
    public int Height => 0;
    public int Width => 0;

    public void Dispose()
    {
    }
}

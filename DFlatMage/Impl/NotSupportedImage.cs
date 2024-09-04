using DFlatMage.Interfaces;
using System.Numerics;

namespace DFlatMage.Impl;

internal class NotSupportedImage : IImage
{
    public int NumPlanes => 0;
    public void Dispose()
    {
    }
}

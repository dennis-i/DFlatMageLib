using DFlatMage.Interfaces;

namespace DFlatMage.Impl;

internal class RawWriter : IImageWriter
{
    public void Write(string filePath, IImage image)
    {
        using FileStream stream = File.OpenWrite(filePath);

        for (int p = 0; p < image.NumPlanes; ++p)
        {
            var buff = image.GetPlane(p);
            stream.Write(buff);
        }

    }
}

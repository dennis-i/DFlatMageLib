using DFlatMage.Impl;

namespace DFlatMage.Interfaces;

public interface IImageWriter
{
    static IImageWriter GetWriter(ImageFormatType type) => new BitmapWriter();


    void Write(string filePath, IImage image);

}

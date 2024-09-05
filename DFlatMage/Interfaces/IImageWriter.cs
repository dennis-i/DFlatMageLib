using DFlatMage.Impl;

namespace DFlatMage.Interfaces;

public interface IImageWriter
{
    static IImageWriter GetWriter(ImageFormatType type) => type switch
    {
        ImageFormatType.Bitmap => new BitmapWriter(),
        ImageFormatType.Raw => new RawWriter()
    };


    void Write(string filePath, IImage image);

}

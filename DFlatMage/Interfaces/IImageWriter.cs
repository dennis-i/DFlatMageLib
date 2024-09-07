using DFlatMage.Enums;
using DFlatMage.Impl.Writers;

namespace DFlatMage.Interfaces;

public interface IImageWriter
{
    static IImageWriter GetWriter(ImageFormatType type) => type switch
    {
        ImageFormatType.Bitmap => new BitmapWriter(),
        ImageFormatType.Raw => new RawWriter(),
        _ => throw new NotImplementedException()
    };


    void Write(string filePath, IImage image);

}

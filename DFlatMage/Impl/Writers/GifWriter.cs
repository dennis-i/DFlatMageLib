using DFlatMage.Interfaces;
using System.Text;

namespace DFlatMage.Impl.Writers;

internal class GifWriter : IImageWriter
{
    public void Write(string filePath, IImage image)
    {
        WriteImage(filePath, image);
    }


    private static void WriteImage(string filePath, IImage image)
    {

        using FileStream stream = File.Create(filePath);

        WriteHeader(stream);
        WriteImage(stream);
        WriteTrailer(stream);
    }

    static void WriteHeader(Stream stream)
    {

        var title = Encoding.ASCII.GetBytes("GIF89a");
        stream.Write(title);


        byte[] screen_descriptor = new byte[]{
            0x64, 0x00, 0x64, 0x00, // Width and height (100x100)
            0xF7,                    // 8 bits per pixel, global color table present
            0x00, 0x00               // Background color index, pixel aspect ratio
        };
        stream.Write(screen_descriptor);


        // Global Color Table (Red, Green, Blue)
        byte[] color_table = {
            0xFF, 0x00, 0x00,       // Red
            0x00, 0xFF, 0x00,       // Green
            0x00, 0x00, 0xFF,       // Blue
            0xFF, 0xFF, 0xFF        // White (for padding)
        };
        stream.Write(color_table);
    }

    static void WriteImage(Stream stream)
    {




        // Compressed data for a 100x100 red image (index 0 in color table)
        byte[] image_data = {
        0x81, 0x00, 0x01, 0x01, // Image data with LZW-encoded red pixels
        0x00                    // Block Terminator
    };




        // Image Descriptor
        byte[] image_descriptor = {
            0x2C,                   // Image Separator
            0x00, 0x00, 0x00, 0x00, // Image position (left, top: 0, 0)
            0x64, 0x00, 0x64, 0x00, // Image size (width, height: 100x100)
            0x00                    // No local color table, no interlace
        };
        stream.Write(image_descriptor);

        // Image Data using LZW Encoding
        stream.WriteByte(0x08); // LZW Minimum Code Size (8 for simplicity)

        stream.Write(image_data);
    }

    static void WriteTrailer(Stream stream)
    {
        stream.WriteByte(0x3B); // GIF trailer
    }

}

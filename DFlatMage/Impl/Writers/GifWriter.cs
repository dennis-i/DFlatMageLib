using DFlatMage.Interfaces;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

        WriteHeader(image, stream);
        WriteImage(image, stream);
        WriteTrailer(stream);
    }


    static void WriteHeader(in IImage img, Stream stream)
    {

        var title = Encoding.ASCII.GetBytes("GIF89a");
        stream.Write(title);




        byte[] screen_descriptor = [
            0x64, 0x00, 0x64, 0x00, // Width and height (100x100)
            0xF7,                    // 8 bits per pixel, global color table present
            0x00, 0x00               // Background color index, pixel aspect ratio
        ];

        var wBytes = BitConverter.GetBytes((ushort)img.Width);
        var hBytes = BitConverter.GetBytes((ushort)img.Height);

        screen_descriptor[0] = wBytes[0];
        screen_descriptor[1] = wBytes[1];
        screen_descriptor[2] = hBytes[0];
        screen_descriptor[3] = hBytes[1];



        stream.Write(screen_descriptor);


        // Global Color Table: 256 entries (3 bytes each), with red as the first color
        byte[] color_table = new byte[256 * 3];
        color_table[0] = 0xFF; // Red color
        for (int i = 3; i < 256 * 3; i += 3)
        {
            color_table[i] = color_table[i + 1] = color_table[i + 2] = 0xFF; // White padding
        }
        stream.Write(color_table);
    }

    static void WriteImage(in IImage img, Stream stream)
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

        var wBytes = BitConverter.GetBytes((ushort)img.Width);
        var hBytes = BitConverter.GetBytes((ushort)img.Height);

        image_descriptor[5] = wBytes[0];
        image_descriptor[6] = wBytes[1];
        image_descriptor[7] = hBytes[0];
        image_descriptor[8] = hBytes[1];

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

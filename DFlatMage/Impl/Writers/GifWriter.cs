using DFlatMage.Interfaces;
using System;
using System.Drawing;
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



    private static void SetNum(Span<byte> bytes, ushort num)
    {
        Span<byte> numspan = BitConverter.GetBytes(num);
        numspan.CopyTo(bytes);
    }

    static void WriteHeader(in IImage img, Stream stream)
    {

        var title = Encoding.ASCII.GetBytes("GIF89a");
        stream.Write(title);




        //Bits 0 - 2    Size of the Global Color Table
        //Bit 3   Color Table Sort Flag
        //Bits 4 - 6    Color Resolution
        //Bit 7   Global Color Table Flag


        int globalColorTableSize = img.Bpp - 1;

        bool colorTableSortFlag = true;
        int colorResolution = img.Bpp - 1;
        bool globalColorTableFlag = true;



        byte backgroundColor = 0;  /* Background Color Index */
        byte aspectRatio = 0;      /* Pixel Aspect Ratio */

        byte packed = (byte)globalColorTableSize;
        packed |= (byte)(colorTableSortFlag ? 0x08 : 0x00);
        packed |= (byte)((0x07 & colorResolution) << 4);
        packed |= (byte)(globalColorTableFlag ? 0x80 : 0x00);

        Span<byte> screen_descriptor = stackalloc byte[7];

        SetNum(screen_descriptor.Slice(0, 2), (ushort)img.Width);
        SetNum(screen_descriptor.Slice(2, 2), (ushort)img.Height);

        screen_descriptor[4] = packed;
        screen_descriptor[5] = backgroundColor;
        screen_descriptor[6] = aspectRatio;




        stream.Write(screen_descriptor);


        // Global Color Table: 256 entries (3 bytes each), with red as the first color

        int numberOfGlobalColorTableEntries = ((int)(1 << (globalColorTableSize + 1)));
        byte[] color_table = new byte[numberOfGlobalColorTableEntries * 3];

        for (int i = 0; i < numberOfGlobalColorTableEntries; i++)
        {
            color_table[i * 3] = (byte)(0xFF - i);
            color_table[i * 3 + 1] = 0xFF;
            color_table[i * 3 + 2] = 0xFF;

        }
        stream.Write(color_table);
    }

    public static Span<byte> LzwCompress(ReadOnlySpan<byte> input)
    {
        // Step 1: Initialize dictionary with single-byte entries
        var dictionary = new Dictionary<string, int>();
        for (int i = 0; i < 256; i++)
            dictionary.Add(((char)i).ToString(), i);

        string current = string.Empty;
        var compressedData = new List<int>();
        int dictSize = 256;

        // Step 2: Process each byte
        foreach (byte symbol in input)
        {
            string combined = current + (char)symbol;
            if (dictionary.ContainsKey(combined))
            {
                current = combined;
            }
            else
            {
                compressedData.Add(dictionary[current]);
                dictionary[combined] = dictSize++;
                current = ((char)symbol).ToString();
            }
        }

        // Add the last current code to compressed data
        if (!string.IsNullOrEmpty(current))
            compressedData.Add(dictionary[current]);


        // Convert the list of integers to a byte array
        using var ms = new MemoryStream();

        foreach (int code in compressedData)
        {
            ms.Write(BitConverter.GetBytes((ushort)code)); // GIF typically uses 12-bit codes
        }
        return ms.GetBuffer();

    }

    static void WriteImage(in IImage img, Stream stream)
    {

        //Bit 0   Local Color Table Flag
        //Bit 1   Interlace Flag
        //Bit 2   Sort Flag
        //Bits 3 - 4    Reserved
        //Bits 5 - 7    Size of Local Color Table Entry


        bool localColorTableFlag = false;
        bool intelaceFlag = false;
        bool sortFlag = true;
        //int localColorTableSize = img.Bpp - 1;
        int localColorTableSize = 0;

        byte packed = (byte)(localColorTableFlag ? 0x01 : 0x00);
        packed |= (byte)(intelaceFlag ? 0x02 : 0x00);
        packed |= (byte)(sortFlag ? 0x04 : 0x00);
        packed |= (byte)((0x07 & localColorTableSize) << 5);
        ushort left = 0;
        ushort top = 0;


        Span<byte> image_descriptor = stackalloc byte[10];

        image_descriptor[0] = 0x2C;

        SetNum(image_descriptor.Slice(1, 2), left);
        SetNum(image_descriptor.Slice(3, 2), top);
        SetNum(image_descriptor.Slice(5, 2), (ushort)img.Width);
        SetNum(image_descriptor.Slice(7, 2), (ushort)img.Height);
        image_descriptor[9] = packed;


        stream.Write(image_descriptor);




        stream.WriteByte(0x08); // LZW Minimum Code Size (8 for simplicity)


        var lzw = LzwCompress(img.GetPlane(0));

        Span<byte> sub = lzw;
        while (!sub.IsEmpty)
        {
            int size = Math.Min(255, sub.Length);
            var data = sub.Slice(0, size);
            stream.WriteByte((byte)size);
            stream.Write(data);
            sub = sub.Slice(size);
        }




        //    // Compressed data for a 100x100 red image (index 0 in color table)
        //    byte[] image_data = {
        //    0x81, 0x00, 0x01, 0x01, // Image data with LZW-encoded red pixels
        //    0x00                    // Block Terminator
        //};


        stream.WriteByte(0x00);

    }

    static void WriteTrailer(Stream stream)
    {
        stream.WriteByte(0x3B); // GIF trailer
    }

}

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
        WriteAppExt(stream);
        WriteImageFrame(new Point(0, 0), image, stream);
        WriteImageFrame(new Point(10, 10), image, stream);
        WriteImageFrame(new Point(20, 10), image, stream);
        WriteImageFrame(new Point(30, 30), image, stream);
        WriteImageFrame(new Point(40, 50), image, stream);
        WriteImageFrame(new Point(30, 40), image, stream);
        WriteImageFrame(new Point(20, 20), image, stream);
        WriteImageFrame(new Point(10, 10), image, stream);


        WriteTrailer(stream);
    }



    private static void SetNum(Span<byte> bytes, ushort num)
    {
        Span<byte> numspan = BitConverter.GetBytes(num);
        numspan.CopyTo(bytes);
    }

    private static void WriteHeader(in IImage img, Stream stream)
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

    private static void WriteAppExt(Stream stream)
    {
        ushort loopCount = 0;

        stream.WriteByte(0x21);                // Extension Introducer
        stream.WriteByte(0xFF);                // Application Extension Label
        stream.WriteByte(0x0B);                // Block Size (11 bytes)

        // Application Identifier ("NETSCAPE2.0")
        stream.Write(Encoding.ASCII.GetBytes("NETSCAPE2.0"), 0, 11);

        stream.WriteByte(0x03);                // Sub-block Size (3 bytes)
        stream.WriteByte(0x01);                // Sub-block ID (Loop Count)

        // Loop Count (0 for infinite loop, or specify any other number)
        stream.WriteByte((byte)(loopCount & 0xFF));       // Low byte
        stream.WriteByte((byte)((loopCount >> 8) & 0xFF)); // High byte

        stream.WriteByte(0x00);                // Block Terminator
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

    static void WriteImageFrame(in Point origin, in IImage img, Stream stream)
    {

        //Bit 0   Local Color Table Flag
        //Bit 1   Interlace Flag
        //Bit 2   Sort Flag
        //Bits 3 - 4    Reserved
        //Bits 5 - 7    Size of Local Color Table Entry


        bool localColorTableFlag = false;
        bool intelaceFlag = false;
        bool sortFlag = false;
        int localColorTableSize = 0;

        byte packed = (byte)(localColorTableFlag ? 0x01 : 0x00);
        packed |= (byte)(intelaceFlag ? 0x02 : 0x00);
        packed |= (byte)(sortFlag ? 0x04 : 0x00);
        packed |= (byte)((0x07 & localColorTableSize) << 5);
        ushort left = (ushort)origin.X;
        ushort top = (ushort)origin.Y;


        Span<byte> image_descriptor = stackalloc byte[10];

        image_descriptor[0] = 0x2C;

        SetNum(image_descriptor.Slice(1, 2), left);
        SetNum(image_descriptor.Slice(3, 2), top);
        SetNum(image_descriptor.Slice(5, 2), (ushort)img.Width);
        SetNum(image_descriptor.Slice(7, 2), (ushort)img.Height);
        image_descriptor[9] = packed;


        stream.Write(image_descriptor);




        stream.WriteByte(0x08); // LZW Minimum Code Size (8 for simplicity)
        GifLzwEncoder enc = new(img.Bpp);

        Span<byte> sub = enc.Encode(img.GetPlane(0));

        while (!sub.IsEmpty)
        {
            int size = Math.Min(255, sub.Length);
            var data = sub.Slice(0, size);
            stream.WriteByte((byte)size);
            stream.Write(data);
            sub = sub.Slice(size);
        }


        stream.WriteByte(0x00);

    }

    static void WriteTrailer(Stream stream)
    {
        stream.WriteByte(0x3B); // GIF trailer
    }

}

public class GifLzwEncoder
{
    private const int ClearCode = 256;
    private const int EoiCode = 257;
    private readonly int initialCodeSize;
    private int codeSize;
    private int nextCode;
    private readonly Dictionary<string, int> dictionary = new();

    public GifLzwEncoder(int bitDepth)
    {
        initialCodeSize = bitDepth + 1;
        ResetDictionary();
    }

    private void ResetDictionary()
    {
        dictionary.Clear();
        for (int i = 0; i < 256; i++)
        {
            dictionary[i.ToString()] = i;
        }
        dictionary[ClearCode.ToString()] = ClearCode;
        dictionary[EoiCode.ToString()] = EoiCode;

        codeSize = initialCodeSize;
        nextCode = EoiCode + 1;
    }




    public byte[] Encode(ReadOnlySpan<byte> input)
    {
        using (var output = new MemoryStream())
        {
            var bitWriter = new BitWriter(output);
            bitWriter.Write(ClearCode, codeSize);

            string currentSequence = input[0].ToString(); // Initialize with the first pixel
            for (int i = 1; i < input.Length; ++i)

            {
                var pixel = input[i];
                string newSequence = currentSequence + "," + pixel;

                if (dictionary.ContainsKey(newSequence))
                {
                    currentSequence = newSequence;
                }
                else
                {
                    bitWriter.Write(dictionary[currentSequence], codeSize);

                    if (nextCode < (1 << codeSize))
                    {
                        dictionary[newSequence] = nextCode++;
                    }
                    else if (codeSize < 12)
                    {
                        codeSize++;
                        dictionary[newSequence] = nextCode++;
                    }
                    else
                    {
                        bitWriter.Write(ClearCode, codeSize);
                        ResetDictionary();
                        codeSize = initialCodeSize;
                        nextCode = EoiCode + 1;
                        dictionary[newSequence] = nextCode++;
                    }

                    currentSequence = pixel.ToString();
                }
            }

            if (!string.IsNullOrEmpty(currentSequence))
            {
                bitWriter.Write(dictionary[currentSequence], codeSize);
            }

            bitWriter.Write(EoiCode, codeSize);
            bitWriter.Flush();

            return output.ToArray();
        }
    }
}

public class BitWriter
{
    private readonly Stream output;
    private int currentByte;
    private int bitPosition;

    public BitWriter(Stream output)
    {
        this.output = output;
    }

    public void Write(int value, int bitCount)
    {
        for (int i = 0; i < bitCount; i++)
        {
            int bit = (value >> i) & 1;
            currentByte |= bit << bitPosition;

            bitPosition++;

            if (bitPosition == 8)
            {
                output.WriteByte((byte)currentByte);
                currentByte = 0;
                bitPosition = 0;
            }
        }
    }

    public void Flush()
    {
        if (bitPosition > 0)
        {
            output.WriteByte((byte)currentByte);
        }
    }
}

//// Example usage:
//public class Program
//{
//    public static void Main()
//    {
//        // Example 8-bit bitmap data
//        byte[] bitmapData = new byte[] { /* Insert raw 8-bit color indices here */ };

//        var encoder = new GifLzwEncoder(bitDepth: 8);
//        byte[] lzwData = encoder.Encode(bitmapData);

//        // The lzwData now contains the LZW-encoded data suitable for use in a GIF file
//        Console.WriteLine("LZW Encoded Data Length: " + lzwData.Length);
//    }
//}


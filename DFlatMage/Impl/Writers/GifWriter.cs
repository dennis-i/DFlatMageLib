using DFlatMage.Interfaces;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Reflection.Emit;
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

    private static Span<byte> ConvertToByteArray(List<int> output)
    {
        const int MaxCodeSize = 12;
        using (var ms = new MemoryStream())
        {
            int currentCodeSize = 9; // Starting code size
            int maxCode = (1 << currentCodeSize) - 1; // Maximum code value for the current code size
            int bitBuffer = 0; // Bit buffer to hold the packed bits
            int bitCount = 0; // Number of bits in the buffer

            foreach (int code in output)
            {
                // Add the new code to the bit buffer
                bitBuffer |= (code << bitCount);
                bitCount += currentCodeSize;

                // While there are at least 8 bits in the buffer, write bytes to the output
                while (bitCount >= 8)
                {
                    ms.WriteByte((byte)(bitBuffer & 0xFF)); // Write the lowest 8 bits
                    bitBuffer >>= 8; // Remove the byte we just wrote
                    bitCount -= 8; // Reduce the bit count
                }

                // If we reach the maximum code value for the current code size, increase the size
                if (code >= maxCode)
                {
                    if (currentCodeSize < MaxCodeSize)
                    {
                        currentCodeSize++;
                        maxCode = (1 << currentCodeSize) - 1; // Update maximum code value
                    }
                }
            }

            // Write any remaining bits in the buffer to the output
            if (bitCount > 0)
            {
                ms.WriteByte((byte)(bitBuffer & 0xFF)); // Write remaining bits
            }

            return ms.ToArray();
        }
    }


    private static Span<byte> LzwEncode(ReadOnlySpan<byte> input)
    {

        const int ClearCode = 256;
        const int EoiCode = 257;



        var dictionary = new Dictionary<string, int>();
        var output = new List<int>();
        int codeSize = 9; // Start with 9 bits
        int nextCode = 258; // Next code value (256 for ClearCode, 257 for EOI)




        void reset()
        {
            codeSize = 9; // Start with 9 bits
            nextCode = 258; // Next code value (256 for ClearCode, 257 for EOI)
            dictionary.Clear();

            // Initialize dictionary with single byte values
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(i.ToString(), i);
            }
        }

        reset();

        string currentSequence = "";
        foreach (var pixel in input)
        {
            string newSequence = currentSequence + pixel;

            // Check if the new sequence exists in the dictionary
            if (dictionary.TryGetValue(newSequence, out int code))
            {
                currentSequence = newSequence; // Continue building the sequence
            }
            else
            {
                // Output the code for the current sequence
                if (dictionary.TryGetValue(currentSequence, out int currentCode))
                {
                    output.Add(currentCode);
                }
                else
                {
                    throw new KeyNotFoundException($"Key '{currentSequence}' not found in dictionary.");
                }

                // Add new sequence to the dictionary
                if (nextCode < (1 << codeSize) && nextCode < 4096)
                {
                    dictionary[newSequence] = nextCode++;
                }

                // Reset if the dictionary is full
                if (nextCode >= (1 << codeSize) || nextCode >= 4096)
                {
                    output.Add(ClearCode); // Clear code to reset the dictionary
                    reset();
                }

                // Start a new sequence with the current pixel
                currentSequence = pixel.ToString();
            }
        }

        // Output the last sequence
        if (!string.IsNullOrEmpty(currentSequence) && dictionary.TryGetValue(currentSequence, out int lastCode))
        {
            output.Add(lastCode);
        }

        // Add End of Information code
        output.Add(EoiCode);

        // Convert to byte array
        return ConvertToByteArray(output);
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

        Span<byte> sub = LzwEncode(img.GetPlane(0));

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


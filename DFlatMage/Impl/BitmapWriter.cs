using DFlatMage.Interfaces;
using System.Text;

namespace DFlatMage.Impl;

internal struct BitmapColorTable
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public int ToInt32 => BitConverter.ToInt32([R, G, B, 0]);
}
internal class BitmapHeader
{
    private const double Resolution = 72;
    public static readonly int HeaderSize = 40 + 14;
    static readonly Func<int, int, int> Align = (what, to) => (what + to - 1) / to * to;
    static readonly Func<int, int> PixPerInchToPixPerM = dpi => (int)Math.Round(dpi / 25.4) * 1000;

    public ReadOnlySpan<byte> HeaderBytes
    {
        get
        {

            var result = new byte[HeaderSize + (ColorTable.Length << 2)];
            int offset = 2;

            Action<int> nextInt32 = v =>
            {
                Buffer.BlockCopy(BitConverter.GetBytes(v), 0, result, offset, 4);
                offset += 4;
            };
            Action<int> nextInt16 = v =>
            {
                ushort vv = (ushort)v;
                Buffer.BlockCopy(BitConverter.GetBytes(vv), 0, result, offset, 2);
                offset += 2;
            };
            var sigBytes = Encoding.ASCII.GetBytes("BM");
            Buffer.BlockCopy(sigBytes, 0, result, 0, sigBytes.Length);

            nextInt32(FileSize);
            nextInt32(Reserved);
            nextInt32(DataOffset);
            nextInt32(Size);
            nextInt32(Width);
            nextInt32(Height);
            nextInt16(Planes);
            nextInt16(Bpp);
            nextInt32(Compression);
            nextInt32(ImageSize);
            nextInt32(XPixelsPerM);
            nextInt32(YPixelsPerM);
            nextInt32(ColorUsed);
            nextInt32(ImportantColors);
            foreach (var ct in ColorTable)
                nextInt32(ct.ToInt32);

            return result;
        }
    }

    public string Signature => "BM";
    public int FileSize { get; set; }
    public int Reserved { get; set; }
    public int DataOffset { get; set; }
    public int Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Planes { get; set; }
    public int Bpp { get; set; }
    public int Compression { get; set; }
    public int ImageSize { get; set; }
    public int XPixelsPerM { get; set; }
    public int YPixelsPerM { get; set; }
    public int ColorUsed { get; set; }
    public int ImportantColors { get; set; }
    public BitmapColorTable[] ColorTable { get; set; } = [];
}

internal class BitmapWriter : IImageWriter
{

    static readonly Func<int, int, int> Align = (what, to) => (what + to - 1) / to * to;
    static readonly Func<int, int> PixPerInchToPixPerM = dpi => (int)Math.Round(dpi / 25.4) * 1000;

    public void Write(string filePath, IImage image)
    {
        const int Resolution = 72;


        int rowSize = image.Width;

        int numBytesForRow = Align(rowSize, 4);
        int padding = numBytesForRow - image.Width;
        int imageDataSize = image.Height * numBytesForRow;

        var header = new BitmapHeader();


        header.ColorTable = Enumerable.Range(0, 256)
            .Select(i => new BitmapColorTable() { R = (byte)(255 - i), B = (byte)(255 - i), G = (byte)(255 - i) })
            .ToArray();

        header.FileSize = BitmapHeader.HeaderSize + header.ColorTable.Length * 4 + imageDataSize;
        header.Reserved = 0;
        header.DataOffset = BitmapHeader.HeaderSize + header.ColorTable.Length * 4;
        header.Size = 40;
        header.Width = image.Width;
        header.Height = image.Height;
        header.Planes = 1;
        header.Bpp = 8;
        header.Compression = 0;
        header.ImageSize = imageDataSize;
        header.XPixelsPerM = PixPerInchToPixPerM(Resolution);
        header.YPixelsPerM = PixPerInchToPixPerM(Resolution);
        header.ColorUsed = 0x100;
        header.ImportantColors = 0;



        using FileStream stream = File.OpenWrite(filePath);

        stream.Write(header.HeaderBytes);
        int lastRow = image.Height - 1;
        for (int row = lastRow; row >= 0; --row)
        {
            Span<byte> rowBuff = image.GetRow(0, row);

            stream.Write(rowBuff);
            stream.Position += padding;
        }


    }
}

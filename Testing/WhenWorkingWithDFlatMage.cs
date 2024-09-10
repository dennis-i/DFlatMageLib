using DFlatMage.Common;
using DFlatMage.Enums;
using DFlatMage.Impl;
using DFlatMage.Interfaces;

namespace Testing;

public class WhenWorkingWithDFlatMage
{


    [Fact]
    public void CreatedImageNotNull()
    {
        using IImage img = IImage.Create(1, 10, 10, Bpp.Bpp8);
        Assert.NotNull(img);
    }

    [Theory]
    [InlineData(1, 2, 3, Bpp.Bpp8)]
    [InlineData(20, 20000, 30000, Bpp.Bpp8)]
    public void CreatedImageHasCorrectDimensions(int nPlanes, int nRows, int nCols, Bpp bpp)
    {
        using IImage img = IImage.Create(nPlanes, nRows, nCols, bpp);
        Assert.Equal(nPlanes, img.NumPlanes);
        Assert.Equal(nRows, img.Height);
        Assert.Equal(nCols, img.Width);
        Assert.Equal((int)bpp, img.Bpp);
    }

    [Theory]
    [InlineData(0, 0, 0, 5)]
    public void SetGetPixel(int p, int x, int y, int val)
    {
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        img.SetPix(p, x, y, val);
        Assert.Equal(val, img.GetPix(p, x, y));
    }


    [Fact]
    public void WrongParametersThrowsException()
    {
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        Assert.Throws<OutOfImageRangeException>(() => { img.SetPix(1, 1, 1, 1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.SetPix(0, 101, 1, 1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.SetPix(0, 0, -1, 1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetPix(1, 1, 1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetPix(0, 101, 1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetPix(0, 0, -1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetPlane(1); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetRow(1, 0); });
        Assert.Throws<OutOfImageRangeException>(() => { img.GetRow(0, 10324); });
    }


    [Fact]
    public void SaveAsBitmap()
    {
        const string filePath = "img.bmp";

        if (File.Exists(filePath))
            File.Delete(filePath);


        const int size = 117;
        using IImage img = IImage.Create(1, size, size, Bpp.Bpp8);

        for (int i = 0; i < size; ++i)
        {
            img.SetPix(0, i, i, 200);
        }


        img.Save(filePath, ImageFormatType.Bitmap);
        Assert.True(File.Exists(filePath));
    }


    [Fact]
    public void SaveAsRgbBitmap()
    {
        using IImage img = IImage.Create(3, 1000, 2000, Bpp.Bpp8);

        const string filePath = "rgb.bmp";

        if (File.Exists(filePath))
            File.Delete(filePath);


        for (int r = 0; r < 10; ++r)
        {
            img.DrawRect(0, new Rect(20 - r, 20 - r, 100 + r * 2, 100 + r * 2), 255);
            img.DrawRect(1, new Rect(20 - r, 20 - r, 100 + r * 2, 100 + r * 2), 0);
            img.DrawRect(2, new Rect(20 - r, 20 - r, 100 + r * 2, 100 + r * 2), 0);
        }



        for (int r = 100; r < 150; ++r)
        {
            img.DrawCirle(0, new Point(400, 400), r, 0);
            img.DrawCirle(1, new Point(400, 400), r, 0);
            img.DrawCirle(2, new Point(400, 400), r, 255);
        }

        img.Save(filePath, ImageFormatType.Bitmap);
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void SaveAsRaw()
    {

        const int size = 800;
        using IImage img = IImage.Create(1, size, size, Bpp.Bpp8);

        string filePath = $"img_{img.NumPlanes}_{img.Width}x{img.Height}.data";

        if (File.Exists(filePath))
            File.Delete(filePath);

        for (int i = 0; i < size; ++i)
        {
            img.SetPix(0, i, i, 200);
        }
        img.Save(filePath, ImageFormatType.Raw);
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void DrawLine()
    {
        const int p = 0;
        const int val = 100;

        using IImage img1 = IImage.Create(1, 100, 100, Bpp.Bpp8);
        img1.DrawLine(p, new Point(50, 50), new Point(10, 10), val);

        using IImage img2 = IImage.Create(1, 100, 100, Bpp.Bpp8);
        img2.DrawLine(p, new Point(10, 10), new Point(50, 50), val);

        img1.Save("lines1.bmp", ImageFormatType.Bitmap);
        img2.Save("lines2.bmp", ImageFormatType.Bitmap);

        Assert.Equal(img1, img2);
    }

    [Fact]
    public void DrawRectangle()
    {
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        img.DrawRect(0, new Rect(5, 5, 20, 40), 200);
        img.Save("rect.bmp", ImageFormatType.Bitmap);
    }

    [Fact]
    public void DrawCircle()
    {
        using IImage img = IImage.Create(1, 1000, 1000, Bpp.Bpp8);

        Point center = new Point(img.Width >> 1, img.Height >> 1);
        for (int r = 100; r < 150; ++r)
            img.DrawCirle(0, center, r, 100 + r);
        img.Save("circle.bmp", ImageFormatType.Bitmap);
    }
}

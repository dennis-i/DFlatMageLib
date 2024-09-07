using DFlatMage.Enums;
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
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        img.DrawLine(0, 10, 1, 50, 50, 200);
        img.Save("line.bmp", ImageFormatType.Bitmap);
    }

    [Fact]
    public void DrawRectangle()
    {
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        img.DrawLine(0, 10, 10, 10, 50, 200);
        img.DrawLine(0, 40, 20, 40, 40, 200);

        img.DrawLine(0, 10, 20, 40, 20, 200);
        img.DrawLine(0, 10, 50, 40, 40, 200);
        img.Save("rect.bmp", ImageFormatType.Bitmap);
    }
}

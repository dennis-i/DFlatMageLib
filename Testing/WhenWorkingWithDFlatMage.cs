using DFlatMage.Interfaces;

namespace Testing;

public class WhenWorkingWithDFlatMage
{


    [Fact]
    public void CreatedImageNotNull()
    {
        using IImage img = IImage.Create(1, 10, 10, 8);
        Assert.NotNull(img);
    }

    [Theory]
    [InlineData(1, 2, 3, 8)]
    [InlineData(20, 20000, 30000, 8)]
    public void CreatedImageHasCorrectDimensions(int nPlanes, int nRows, int nCols, int bpp)
    {
        using IImage img = IImage.Create(nPlanes, nRows, nCols, bpp);
        Assert.Equal(nPlanes, img.NumPlanes);
        Assert.Equal(nRows, img.Height);
        Assert.Equal(nCols, img.Width);
    }

    [Theory]
    [InlineData(0, 0, 0, 5)]
    public void SetGetPixel(int p, int x, int y, int val)
    {
        using IImage img = IImage.Create(1, 100, 100, 8);

        img.SetPix(p, x, y, val);
        Assert.Equal(val, img.GetPix(p, x, y));
    }

    [Fact]
    public void SaveAsBitmap()
    {
        const string filePath = "img.bmp";

        using IImage img = IImage.Create(1, 100, 100, 8);
        img.Save(filePath, ImageFormatType.Bitmap);
    }
}

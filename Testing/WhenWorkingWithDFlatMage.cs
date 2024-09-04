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
    public void CreatedImageHasCorrectDimensions(int nPlanes,int nRows,int nCols,int bpp)
    {
        using IImage img = IImage.Create(nPlanes, nRows, nCols, bpp);
        Assert.Equal(nPlanes, img.NumPlanes);
    }
}

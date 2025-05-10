using DFlatMage.Common;
using DFlatMage.Enums;
using DFlatMage.Interfaces;

namespace Testing;

public class WhenWorkingWithRect:TestBase
{
    [Fact]
    public void OverlapRect()
    {
        var r1 = new Rect(0, 0, 10, 10);
        var r2 = new Rect(5, 5, 10, 10);

        Rect ov = r1.OverlappedRect(r2);
        Assert.Equal(new(5, 5, 5, 5), ov);


        r1 = new Rect(0, 0, 15, 15);
        r2 = new Rect(5, 5, 10, 10);

        ov = r1.OverlappedRect(r2);
        Assert.Equal(new(5, 5, 10, 10), ov);
        
        
        r1 = new Rect(0, 0, 10, 5);
        r2 = new Rect(2, 2, 4, 4);

        ov = r1.OverlappedRect(r2);
        Assert.Equal(new(2, 2, 4, 3), ov);
        
        
        r2 = new Rect(0, 0, 10, 5);
        r1 = new Rect(2, 2, 4, 4);

        ov = r1.OverlappedRect(r2);
        Assert.Equal(new(2, 2, 4, 3), ov);
    }


    [Theory]
    [InlineData(false, 0, 0, 1, 1, 1, 1)]
    [InlineData(true, 0, 0, 0, 0, 1, 1)]
    [InlineData(false, 0, 2, 0, 0, 1, 1)]
    [InlineData(false, 2, 0, 0, 0, 1, 1)]
    public void PointInside(bool expected, int ptX, int ptY, int rectX, int rectY, int rectWidth, int rectHeight)
    {
        Rect r = new(rectX, rectY, rectWidth, rectHeight);
        Point p = new(ptX, ptY);
        Assert.Equal(expected, p.Inside(r));
    }

    [Theory]
    [InlineData(1, 0, 0, 100, 50, 20, 20,40,40)]
    [InlineData(2, 0, 0, 100, 50, 10, 20,40,40)]
    public void DrawOverlappingRect(int fileN,int r1X,int r1Y,int r1W,int r1H,int r2X,int r2Y,int r2W,int r2H)
    {
         string filePath = $"overlapping{fileN}.bmp";
        using IImage img  = IImage.Create(1,200,200,Bpp.Bpp8);
        
        var r1 = new Rect(r1X, r1Y, r1W, r1H);
        var r2 = new Rect(r2X, r2Y, r2W, r2H);
        var ov = r1.OverlappedRect(r2);
        img.DrawRect(0,r1,127);
        img.DrawRect(0,r2,127);
        img.DrawRect(0,ov,220);
        
        ImageSaveBmp(img, filePath);
    }
}
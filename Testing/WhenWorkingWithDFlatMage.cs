﻿using DFlatMage.Common;
using DFlatMage.Enums;
using DFlatMage.Impl;
using DFlatMage.Interfaces;

namespace Testing;

public class WhenWorkingWithDFlatMage : TestBase
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

        if (File.Exists(Path.Combine(ArtifactsPath, filePath)))
            File.Delete(Path.Combine(ArtifactsPath, filePath));


        const int size = 117;
        using IImage img = IImage.Create(1, size, size, Bpp.Bpp8);

        for (int i = 0; i < size; ++i)
        {
            img.SetPix(0, i, i, 200);
        }

        ImageSaveBmp(img, filePath);
        Assert.True(File.Exists(Path.Combine(ArtifactsPath, filePath)));
    }


    [Fact]
    public void SaveAsRgbBitmap()
    {
        using IImage img = IImage.Create(3, 1000, 1000, Bpp.Bpp8);

        const string filePath = "rgb.bmp";

        if (File.Exists(Path.Combine(ArtifactsPath, filePath)))
            File.Delete(Path.Combine(ArtifactsPath, filePath));

        for (int i = 20; i < 200; i += 25)
        {
            for (int r = 0; r < 10; ++r)
            {
                img.DrawRect(..3, new Rect(i - r, i - r, 100 + r * 2, 100 + r * 2), [255, 0, 0]);
            }
        }


        for (int r = 100; r < 140; ++r)
        {
            img.DrawCircle(..3, new Point(400, 400), r, [0, 0, 255]);
            img.DrawCircle(..3, new Point(700, 700), r, [180, 0, 255]);
            img.DrawCircle(..3, new Point(700, 400), r, [180, 255, 0]);
            img.DrawCircle(..3, new Point(400, 700), r, [0, 200, 255]);

        }
        ImageSaveBmp(img, filePath);
        Assert.True(File.Exists(Path.Combine(ArtifactsPath, filePath)));


        using IImage scaled = img.Scale(5, 4);
        ImageSaveBmp(scaled, "scaled_rgb.bmp");

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
        img2.DrawLine(p, new Point(50, 50), new Point(10, 10), val);


        ImageSaveBmp(img1, "lines1.bmp");
        ImageSaveBmp(img2, "lines2.bmp");
        Assert.Equal(img1, img2);
    }

    [Theory]
    [InlineData(0, 0, 50, 50)]
    [InlineData(0, 0, 50, 0)]
    [InlineData(0, 0, 0, 50)]

    [InlineData(50, 50, 0, 0)]
    [InlineData(50, 50, 0, 50)]
    [InlineData(50, 50, 50, 0)]

    [InlineData(50, 50, 80, 20)]

    [InlineData(57, 98, 99, 74)]
    public void DrawLines(int x1, int y1, int x2, int y2)
    {
        const int p = 0;
        const int val = 100;

        Point start = new(x1, y1);
        Point end = new(x2, y2);


        using IImage img1 = IImage.Create(1, 100, 100, Bpp.Bpp8);
        img1.DrawLine(p, start, end, val);

        using IImage img2 = IImage.Create(1, 100, 100, Bpp.Bpp8);
        img2.DrawLine(p, start, end, val);


        ImageSaveBmp(img1, "lines1.bmp");
        ImageSaveBmp(img2, "lines2.bmp");

        Assert.Equal(img1, img2);
    }

    [Fact]
    public void DrawRectangle()
    {
        using IImage img = IImage.Create(1, 100, 100, Bpp.Bpp8);

        img.DrawRect(0, new Rect(5, 5, 20, 40), 200);
        ImageSaveBmp(img, "rect.bmp");
    }

    [Fact]
    public void DrawCircle()
    {
        using IImage img = IImage.Create(1, 1000, 1000, Bpp.Bpp8);

        Point center = new Point(img.Width >> 1, img.Height >> 1);
        for (int r = 100; r < 150; ++r)
            img.DrawCircle(0, center, r, 100 + r);

        ImageSaveBmp(img, "circle.bmp");

    }


    [Fact]
    public void DrawSnowFlake()
    {
        const int size = 4000;
        using IImage img = IImage.Create(3, size, size, Bpp.Bpp8);

        int numEdges = 17;
        int levels = 5;
        int edgeLen = (int)(size / 7.5);

        Point center = new(img.Width >> 1, img.Height >> 1);

        DrawFromCenter(img, center, edgeLen, levels, numEdges);

        ImageSaveBmp(img, "snowflake.bmp");
        using IImage scaled = img.Scale(2, 2);
        using IImage scaled1 = scaled.Scale(0.5, 0.5);

        ImageSaveBmp(scaled1, "snowflake_scaled.bmp");

    }

    private void DrawFromCenter(IImage img, Point center, int edgeLen, int level, int numEdges)
    {
        var angle = 2.0 * Math.PI / numEdges;

        if (level == 0)
            return;

        int[][] color = [
            [255, 0, 0],
            [0, 255, 0],
            [0, 0, 255],
            [255, 255,0],
            [255, 0,255],
        ];

        for (int i = 0; i < numEdges; ++i)
        {
            var edge = new Point((int)(center.X + Math.Cos(angle * (i + level * 0.1)) * edgeLen),
                                 (int)(center.Y + Math.Sin(angle * (i + level * 0.1)) * edgeLen));



            img.DrawLine(0..3, center, edge, color[level % color.Length]);

            DrawFromCenter(img, edge, (int)(edgeLen * 0.75), level - 1, numEdges);
        }
    }

    [Fact]
    public void DrawPath()
    {

        using IImage img1 = IImage.Create(1, 400, 400, Bpp.Bpp8);
        using IImage img2 = IImage.Create(1, 400, 400, Bpp.Bpp8);

        const int numEdges = 50;

        Point[] path = new Point[numEdges];
        for (int i = 0; i < path.Length; ++i)
        {
            int x = Random.Shared.Next(0, img2.Width);
            int y = Random.Shared.Next(0, img2.Height);
            path[i] = new Point(x, y);
        }
        img1.DrawPath(0, path, 200);
        img2.DrawPath(0, path, 200);

        ImageSaveBmp(img1, "path1.bmp");
        ImageSaveBmp(img2, "path2.bmp");



        Assert.Equal(img1, img2);

    }


    [Fact]
    public void Crop()
    {
        const int thic = 52;
        using IImage img = IImage.Create(3, 400, 400, Bpp.Bpp8);
        var center = new Point(img.Width >> 1, img.Height >> 1);
        for (int i = 0; i < thic; ++i)
            img.DrawCircle(0..3, center, 150 - i, [150 - i, 0, 0]);


        ImageSaveBmp(img, "full.bmp");

        using IImage cropped = img.Crop(new Rect(0, 0, 200, 200));
        ImageSaveBmp(cropped, "cropped.bmp");

    }

    [Fact]
    public void Scale()
    {
        using IImage img = IImage.Create(3, 50, 50, Bpp.Bpp8);

        for (int i = 0; i < 15; ++i)
        {
            img.DrawRect(0..3, new Rect(5 + i, 5 + i, 40 - i * 2, 40 - i * 2), [0, 100 + i * 10, 0]);
        }
        ImageSaveBmp(img, "sqare.bmp");


        using IImage scaled = img.Scale(2.5, 2.5);

        ImageSaveBmp(scaled, "scaled.bmp");


    }

}

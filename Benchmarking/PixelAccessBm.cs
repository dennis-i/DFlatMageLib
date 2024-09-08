using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using DFlatMage.Enums;
using DFlatMage.Interfaces;

namespace Benchmarking;

//[SimpleJob(RunStrategy.ColdStart, iterationCount: 500)]
//[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
public class PixelAccessBm
{
    private IImage? image;

    [GlobalSetup]
    public void Setup()
    {
        image = IImage.Create(1, 1000, 1000, Bpp.Bpp8);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        image?.Dispose();
    }

    [Benchmark]
    [Arguments(0, 0)]
    [Arguments(1, 1)]
    [Arguments(200, 123)]
    [Arguments(123, 567)]
    public void SetPixel(int row, int col)
    {
        image!.SetPix(0, row, col, 200);
    }

    [Benchmark]
    [Arguments(0, 0)]
    [Arguments(1, 1)]
    [Arguments(200, 123)]
    [Arguments(123, 567)]
    public int GetPixel(int row, int col)
    {
        return image!.GetPix(0, row, col);
    }
}
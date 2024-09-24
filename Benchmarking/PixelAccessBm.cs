using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Benchmarking;

//[SimpleJob(RunStrategy.ColdStart, iterationCount: 500)]
//[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
public class PixelAccessBm: BenchmarkBase
{

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


    [Benchmark]
    [Arguments(0, 0)]
    [Arguments(1, 1)]
    [Arguments(200, 123)]
    [Arguments(123, 567)]
    public void SetPixelUnsafe(int row, int col)
    {
        image!.SetPixUnsafe(0, row, col, 200);
    }

    [Benchmark]
    [Arguments(0, 0)]
    [Arguments(1, 1)]
    [Arguments(200, 123)]
    [Arguments(123, 567)]
    public int GetPixelUnsafe(int row, int col)
    {
        return image!.GetPixUnsafe(0, row, col);
    }
}
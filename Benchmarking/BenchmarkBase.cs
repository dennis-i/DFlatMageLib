using BenchmarkDotNet.Attributes;
using DFlatMage.Enums;
using DFlatMage.Interfaces;

namespace Benchmarking;

public abstract class BenchmarkBase
{
    protected IImage? image;

    [GlobalSetup]
    public void Setup() => image = IImage.Create(1, 1000, 1000, Bpp.Bpp8);



    [GlobalCleanup]
    public void Cleanup() => image?.Dispose();

}
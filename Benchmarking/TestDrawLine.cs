using BenchmarkDotNet.Attributes;
using DFlatMage.Common;

namespace Benchmarking;

//[ShortRunJob]
public class TestDrawLine : BenchmarkBase
{

    [Benchmark]
    public void DrawLine()
    {
        image?.DrawLine(0, new Point(0, 0), new Point(image.Width - 1, image.Height - 1), 180);
    }

}
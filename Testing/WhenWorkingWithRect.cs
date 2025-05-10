using DFlatMage.Common;

namespace Testing;

public class WhenWorkingWithRect
{
    [Fact]
    public void OverlapRect()
    {
        var r1 = new Rect(0,0, 10, 10);
        var r2 = new Rect(5, 5, 10, 10);

        Rect ov= r1.OverlapWith(r2);
        Assert.Equal(new(5,5,5,5), ov);
    }

}
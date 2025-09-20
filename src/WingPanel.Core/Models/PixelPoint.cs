namespace WingPanel.Core.Models;

public readonly record struct PixelPoint(int X, int Y)
{
    public static PixelPoint Empty { get; } = new(0, 0);
}

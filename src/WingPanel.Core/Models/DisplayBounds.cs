namespace WingPanel.Core.Models;

public readonly record struct DisplayBounds(int X, int Y, int Width, int Height)
{
    public static DisplayBounds Default { get; } = new(0, 0, 1920, 1080);

    public int Right => X + Width;

    public int Bottom => Y + Height;
}

using System;

namespace WingPanel.Core.Models;

public sealed record WindowInfo(nint Handle, string Title, bool IsVisible)
{
    public DateTime LastActivatedUtc { get; init; } = DateTime.UtcNow;

    public WindowInfo WithActivatedNow() => this with { LastActivatedUtc = DateTime.UtcNow };
}

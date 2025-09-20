using System;
using System.Threading;
using WingPanel.Core.Interop;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public sealed class DisplayBoundsService : IDisplayBoundsService, IDisposable
{
    private readonly Timer _pollTimer;
    private DisplayBounds _current;

    public DisplayBoundsService()
    {
        _current = QueryBounds();
        _pollTimer = new Timer(OnPoll, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
    }

    public event EventHandler<DisplayBounds>? DisplayChanged;

    public DisplayBounds GetPrimaryDisplay() => _current;

    private DisplayBounds QueryBounds()
    {
        if (OperatingSystem.IsWindows())
        {
            var width = User32.GetSystemMetrics(User32.SM_CXSCREEN);
            var height = User32.GetSystemMetrics(User32.SM_CYSCREEN);
            if (width > 0 && height > 0)
            {
                return new DisplayBounds(0, 0, width, height);
            }
        }

        return DisplayBounds.Default;
    }

    private void OnPoll(object? state)
    {
        var bounds = QueryBounds();
        if (!bounds.Equals(_current))
        {
            _current = bounds;
            DisplayChanged?.Invoke(this, bounds);
        }
    }

    public void Dispose()
    {
        _pollTimer.Dispose();
    }
}

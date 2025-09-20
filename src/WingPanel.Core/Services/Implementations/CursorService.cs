using System;
using System.Threading;
using WingPanel.Core.Interop;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public sealed class CursorService : ICursorService, IDisposable
{
    private readonly Timer _pollTimer;
    private PixelPoint _lastKnown = PixelPoint.Empty;

    public CursorService()
    {
        _pollTimer = new Timer(OnPoll, null, TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250));
    }

    public event EventHandler<PixelPoint>? CursorMoved;

    public PixelPoint GetCursorPosition()
    {
        var point = QueryCursor();
        _lastKnown = point;
        return point;
    }

    public void SetCursorPosition(PixelPoint point)
    {
        _lastKnown = point;
        if (OperatingSystem.IsWindows())
        {
            User32.SetCursorPos(point.X, point.Y);
        }

        CursorMoved?.Invoke(this, point);
    }

    private PixelPoint QueryCursor()
    {
        if (OperatingSystem.IsWindows() && User32.GetCursorPos(out var point))
        {
            return new PixelPoint(point.X, point.Y);
        }

        return _lastKnown;
    }

    private void OnPoll(object? state)
    {
        var point = QueryCursor();
        if (point != _lastKnown)
        {
            _lastKnown = point;
            CursorMoved?.Invoke(this, point);
        }
    }

    public void Dispose()
    {
        _pollTimer.Dispose();
    }
}

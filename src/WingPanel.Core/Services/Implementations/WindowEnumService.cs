using System;
using System.Collections.Generic;
using System.Linq;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Interfaces;
using WingPanel.Core.Utilities;

namespace WingPanel.Core.Services.Implementations;

public interface IWindowEnumerationBackend
{
    IEnumerable<WindowInfo> EnumerateWindows();
}

internal sealed class DefaultWindowEnumerationBackend : IWindowEnumerationBackend
{
    public IEnumerable<WindowInfo> EnumerateWindows()
    {
        yield break;
    }
}

public sealed class WindowEnumService : IWindowEnumService
{
    private readonly IWindowEnumerationBackend _backend;
    private readonly MruList<WindowInfo> _mru = new(new WindowInfoHandleComparer());
    private List<WindowInfo> _current = new();

    public WindowEnumService(IWindowEnumerationBackend? backend = null)
    {
        _backend = backend ?? new DefaultWindowEnumerationBackend();
        Refresh();
    }

    public event EventHandler<IReadOnlyList<WindowInfo>>? WindowsChanged;

    public IReadOnlyList<WindowInfo> CurrentWindows => _current;

    public void Refresh()
    {
        var enumerated = _backend.EnumerateWindows().Where(w => w.IsVisible).ToList();
        foreach (var window in enumerated)
        {
            _mru.Touch(window);
        }

        Rebuild(enumerated, raiseEvent: true);
    }

    public void NotifyActivated(WindowInfo window)
    {
        var activated = window.WithActivatedNow();
        _mru.Touch(activated);
        Rebuild(null, raiseEvent: true);
    }

    private void Rebuild(IReadOnlyCollection<WindowInfo>? enumerated, bool raiseEvent)
    {
        var source = enumerated?.ToDictionary(w => w.Handle) ?? _current.ToDictionary(w => w.Handle);
        var ordered = new List<WindowInfo>();

        foreach (var item in _mru)
        {
            if (source.TryGetValue(item.Handle, out var existing))
            {
                ordered.Add(existing with { LastActivatedUtc = item.LastActivatedUtc });
            }
        }

        if (enumerated is not null)
        {
            foreach (var window in enumerated)
            {
                if (!ordered.Any(w => w.Handle == window.Handle))
                {
                    ordered.Add(window);
                }
            }
        }

        _current = ordered;

        if (raiseEvent)
        {
            WindowsChanged?.Invoke(this, _current);
        }
    }
}

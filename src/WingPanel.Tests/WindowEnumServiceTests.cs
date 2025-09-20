using System;
using System.Collections.Generic;
using System.Linq;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Implementations;
using Xunit;

namespace WingPanel.Tests;

public class WindowEnumServiceTests
{
    [Fact]
    public void Refresh_OrdersByMru()
    {
        var backend = new FakeWindowBackend(
            new WindowInfo(1, "First", true),
            new WindowInfo(2, "Second", true),
            new WindowInfo(3, "Third", true));
        var service = new WindowEnumService(backend);

        Assert.Equal(new[] { 1, 2, 3 }, service.CurrentWindows.Select(w => (int)w.Handle));

        service.NotifyActivated(new WindowInfo(2, "Second", true));

        Assert.Equal(new[] { 2, 1, 3 }, service.CurrentWindows.Select(w => (int)w.Handle));
    }

    [Fact]
    public void Refresh_AddsNewWindowsToEnd()
    {
        var backend = new FakeWindowBackend(
            new WindowInfo(1, "First", true),
            new WindowInfo(2, "Second", true));
        var service = new WindowEnumService(backend);

        backend.SetWindows(new WindowInfo(1, "First", true), new WindowInfo(2, "Second", true), new WindowInfo(3, "Third", true));
        service.Refresh();

        Assert.Equal(new[] { 1, 2, 3 }, service.CurrentWindows.Select(w => (int)w.Handle));
    }

    private sealed class FakeWindowBackend : IWindowEnumerationBackend
    {
        private IReadOnlyList<WindowInfo> _windows;

        public FakeWindowBackend(params WindowInfo[] windows)
        {
            _windows = windows;
        }

        public IEnumerable<WindowInfo> EnumerateWindows() => _windows;

        public void SetWindows(params WindowInfo[] windows) => _windows = windows;
    }
}

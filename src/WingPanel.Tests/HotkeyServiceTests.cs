using System.Collections.Generic;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Implementations;
using WingPanel.Core.Services.Interfaces;
using Xunit;

namespace WingPanel.Tests;

public class HotkeyServiceTests
{
    [Fact]
    public void Register_WinSpaceFallsBackToCtrlWinSpace()
    {
        var backend = new RejectingBackend();
        var service = new HotkeyService(backend, new NoOpLogService());
        var success = service.TryRegister("Toggle", new HotkeyBinding(HotkeyModifiers.Win, VirtualKey.Space));

        Assert.True(success);
        Assert.True(service.Registrations.TryGetValue("Toggle", out var registered));
        Assert.Equal(HotkeyModifiers.Win | HotkeyModifiers.Control, registered.Modifiers);
        Assert.Equal(VirtualKey.Space, registered.Key);
        Assert.Equal("Toggle", backend.LastRegisteredId);
        Assert.Equal(registered, backend.LastRegisteredBinding);
    }

    [Fact]
    public void Register_CustomHotkeySuccess()
    {
        var backend = new RejectingBackend();
        backend.Allowed.Add(new HotkeyBinding(HotkeyModifiers.Control | HotkeyModifiers.Shift, VirtualKey.A));
        var service = new HotkeyService(backend, new NoOpLogService());

        Assert.True(service.TryRegister("Custom", new HotkeyBinding(HotkeyModifiers.Control | HotkeyModifiers.Shift, VirtualKey.A)));
        Assert.Equal(HotkeyModifiers.Control | HotkeyModifiers.Shift, service.Registrations["Custom"].Modifiers);
    }

    private sealed class RejectingBackend : IHotkeyRegistrationBackend
    {
        public HashSet<HotkeyBinding> Allowed { get; } = new();

        public HotkeyBinding? LastRegisteredBinding { get; private set; }

        public string? LastRegisteredId { get; private set; }

        public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

        public bool TryRegister(string name, HotkeyBinding binding)
        {
            if (Allowed.Contains(binding))
            {
                LastRegisteredId = name;
                LastRegisteredBinding = binding;
                return true;
            }

            if (binding.Modifiers.HasFlag(HotkeyModifiers.Control) && binding.Modifiers.HasFlag(HotkeyModifiers.Win) && binding.Key == VirtualKey.Space)
            {
                Allowed.Add(binding);
                LastRegisteredId = name;
                LastRegisteredBinding = binding;
                return true;
            }

            return false;
        }

        public void Unregister(string name)
        {
        }

        public void Reset()
        {
            Allowed.Clear();
        }
    }

    private sealed class NoOpLogService : ILogService
    {
        public void Log(Microsoft.Extensions.Logging.LogLevel level, string message, Exception? exception = null)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public interface IHotkeyRegistrationBackend
{
    event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    bool TryRegister(string name, HotkeyBinding binding);

    void Unregister(string name);

    void Reset();
}

internal sealed class InMemoryHotkeyBackend : IHotkeyRegistrationBackend
{
    private readonly Dictionary<string, HotkeyBinding> _bindings = new(StringComparer.OrdinalIgnoreCase);

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public IReadOnlyDictionary<string, HotkeyBinding> Registrations => _bindings;

    public bool TryRegister(string name, HotkeyBinding binding)
    {
        _bindings[name] = binding;
        return true;
    }

    public void Unregister(string name)
    {
        _bindings.Remove(name);
    }

    public void Reset()
    {
        _bindings.Clear();
    }

    public void Raise(string name)
    {
        if (_bindings.TryGetValue(name, out var binding))
        {
            HotkeyPressed?.Invoke(this, new HotkeyEventArgs(name, binding));
        }
    }
}

public sealed class HotkeyService : IHotkeyService
{
    private readonly IHotkeyRegistrationBackend _backend;
    private readonly ILogService? _logger;
    private readonly Dictionary<string, HotkeyBinding> _registrations = new(StringComparer.OrdinalIgnoreCase);

    public HotkeyService(IHotkeyRegistrationBackend? backend = null, ILogService? logger = null)
    {
        _backend = backend ?? new InMemoryHotkeyBackend();
        _logger = logger;
        _backend.HotkeyPressed += OnBackendHotkeyPressed;
    }

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public IReadOnlyDictionary<string, HotkeyBinding> Registrations => _registrations;

    public bool TryRegister(string name, HotkeyBinding binding)
    {
        if (TryRegisterInternal(name, binding))
        {
            return true;
        }

        if (binding.Modifiers.HasFlag(HotkeyModifiers.Win) && binding.Key == VirtualKey.Space)
        {
            var fallback = binding.WithAdditionalModifier(HotkeyModifiers.Control);
            if (TryRegisterInternal(name, fallback))
            {
                _logger?.LogInformation($"Applied fallback Ctrl modifier for {name}");
                return true;
            }
        }

        _logger?.LogWarning($"Failed to register hotkey {name} ({binding})");
        return false;
    }

    public void Unregister(string name)
    {
        if (_registrations.Remove(name))
        {
            _backend.Unregister(name);
        }
    }

    public void Reset()
    {
        _backend.Reset();
        _registrations.Clear();
    }

    private bool TryRegisterInternal(string name, HotkeyBinding binding)
    {
        if (_backend.TryRegister(name, binding))
        {
            _registrations[name] = binding;
            _logger?.LogInformation($"Registered {name} => {binding}");
            return true;
        }

        return false;
    }

    private void OnBackendHotkeyPressed(object? sender, HotkeyEventArgs e)
    {
        HotkeyPressed?.Invoke(this, e);
    }
}

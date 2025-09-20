using System;
using System.Collections.Generic;
using WingPanel.Core.Models;

namespace WingPanel.Core.Services.Interfaces;

public interface IHotkeyService
{
    event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    IReadOnlyDictionary<string, HotkeyBinding> Registrations { get; }

    bool TryRegister(string name, HotkeyBinding binding);

    void Unregister(string name);

    void Reset();
}

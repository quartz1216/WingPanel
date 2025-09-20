using System;
using System.Text.Json.Serialization;

namespace WingPanel.Core.Models;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HotkeyModifiers
{
    None = 0,
    Control = 1,
    Shift = 2,
    Alt = 4,
    Win = 8
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VirtualKey
{
    None = 0,
    A = 0x41,
    C = 0x43,
    V = 0x56,
    Space = 0x20,
    Escape = 0x1B,
    Tab = 0x09,
    Left = 0x25,
    Right = 0x27,
    Up = 0x26,
    Down = 0x28,
    Enter = 0x0D
}

public readonly record struct HotkeyBinding(HotkeyModifiers Modifiers, VirtualKey Key)
{
    public static HotkeyBinding WinSpace { get; } = new(HotkeyModifiers.Win, VirtualKey.Space);

    public bool IsEmpty => Modifiers == HotkeyModifiers.None && Key == VirtualKey.None;

    public HotkeyBinding WithModifiers(HotkeyModifiers modifiers) => new(modifiers, Key);

    public HotkeyBinding WithAdditionalModifier(HotkeyModifiers modifier)
        => new(Modifiers | modifier, Key);

    public override string ToString() => $"{Modifiers}+{Key}";
}

public sealed class HotkeyEventArgs : EventArgs
{
    public HotkeyEventArgs(string name, HotkeyBinding binding)
    {
        Name = name;
        Binding = binding;
    }

    public string Name { get; }

    public HotkeyBinding Binding { get; }
}

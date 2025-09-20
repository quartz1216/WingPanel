using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WingPanel.Core.Models.Settings;

using WingPanel.Core.Models;

public class WingPanelSettings
{
    public const int CurrentVersion = 2;

    public int Version { get; set; } = CurrentVersion;

    public HotkeyBinding ToggleHotkey { get; set; } = HotkeyBinding.WinSpace;

    public LauncherSettings Launcher { get; set; } = new();

    public VolumeSettings Volume { get; set; } = new();

    public TaskSwitcherSettings TaskSwitcher { get; set; } = new();

    public NotificationSettings Notifications { get; set; } = new();

    public RegionLayoutSettings Layout { get; set; } = RegionLayoutSettings.CreateDefault();

    [JsonExtensionData]
    public Dictionary<string, object?> ExtensionData { get; set; } = new();
}

public class LauncherSettings
{
    public bool ShowFrequentApps { get; set; } = true;

    public int MaxFrequentApps { get; set; } = 6;
}

public class VolumeSettings
{
    public bool ShowDeviceSwitcher { get; set; } = true;

    public bool UseHardwareButtons { get; set; } = true;
}

public class TaskSwitcherSettings
{
    public bool ShowDesktopPreview { get; set; } = true;

    public bool IncludeMinimizedWindows { get; set; } = true;
}

public class NotificationSettings
{
    public bool ShowBadges { get; set; } = true;

    public bool EnableDoNotDisturb { get; set; } = false;
}

public class RegionLayoutSettings
{
    public List<string> OrderedRegions { get; set; } = new()
    {
        RegionNames.Launcher,
        RegionNames.TaskSwitcher,
        RegionNames.Volume,
        RegionNames.Notifications
    };

    public static RegionLayoutSettings CreateDefault() => new();
}

public static class RegionNames
{
    public const string Launcher = "Launcher";
    public const string Volume = "Volume";
    public const string TaskSwitcher = "TaskSwitcher";
    public const string Notifications = "Notifications";
}

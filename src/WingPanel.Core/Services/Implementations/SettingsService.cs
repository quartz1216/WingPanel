using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WingPanel.Core.Models.Settings;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public sealed class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly string _settingsPath;
    private readonly ILogService? _logger;

    public SettingsService(ILogService? logger = null, string? baseDirectory = null)
    {
        _logger = logger;
        var root = baseDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WingPanel");
        Directory.CreateDirectory(root);
        _settingsPath = Path.Combine(root, "Settings.json");
        Current = LoadFromDisk();
    }

    public event EventHandler<WingPanelSettings>? SettingsChanged;

    public WingPanelSettings Current { get; private set; }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var json = JsonSerializer.Serialize(Current, Options);
            await File.WriteAllTextAsync(_settingsPath, json, cancellationToken).ConfigureAwait(false);
            _logger?.LogInformation($"Saved settings to {_settingsPath}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            Current = LoadFromDisk();
        }
        finally
        {
            _semaphore.Release();
        }

        SettingsChanged?.Invoke(this, Current);
    }

    private WingPanelSettings LoadFromDisk()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                _logger?.LogInformation("Settings file not found; using defaults");
                return EnsureDefaults(new WingPanelSettings());
            }

            var json = File.ReadAllText(_settingsPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger?.LogWarning("Settings file empty; using defaults");
                return EnsureDefaults(new WingPanelSettings());
            }

            var settings = JsonSerializer.Deserialize<WingPanelSettings>(json, Options) ?? new WingPanelSettings();
            return EnsureDefaults(settings);
        }
        catch (Exception ex)
        {
            _logger?.LogError("Failed to load settings; using defaults", ex);
            return EnsureDefaults(new WingPanelSettings());
        }
    }

    private static WingPanelSettings EnsureDefaults(WingPanelSettings settings)
    {
        settings.Layout ??= RegionLayoutSettings.CreateDefault();
        settings.Launcher ??= new LauncherSettings();
        settings.Volume ??= new VolumeSettings();
        settings.TaskSwitcher ??= new TaskSwitcherSettings();
        settings.Notifications ??= new NotificationSettings();
        if (settings.Version < WingPanelSettings.CurrentVersion)
        {
            if (settings.Layout.OrderedRegions.Count == 0)
            {
                settings.Layout = RegionLayoutSettings.CreateDefault();
            }

            settings.Version = WingPanelSettings.CurrentVersion;
        }

        return settings;
    }
}

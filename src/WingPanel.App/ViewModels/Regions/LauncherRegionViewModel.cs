using System.Collections.ObjectModel;
using WingPanel.Core.Models.Settings;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.App.ViewModels.Regions;

public sealed class LauncherRegionViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly ILogService _logger;
    private string _title = "Launcher";

    public LauncherRegionViewModel(ISettingsService settings, ILogService logger)
    {
        _settings = settings;
        _logger = logger;
        FrequentApps = new ObservableCollection<string>();
        ApplySettings(settings.Current);
        _settings.SettingsChanged += OnSettingsChanged;
    }

    public ObservableCollection<string> FrequentApps { get; }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private void OnSettingsChanged(object? sender, WingPanelSettings settings)
    {
        ApplySettings(settings);
    }

    private void ApplySettings(WingPanelSettings settings)
    {
        FrequentApps.Clear();
        if (settings.Launcher.ShowFrequentApps)
        {
            for (var i = 1; i <= settings.Launcher.MaxFrequentApps; i++)
            {
                FrequentApps.Add($"Frequent app {i}");
            }
            Title = "Frequent apps";
        }
        else
        {
            Title = "Launcher";
        }

        _logger.LogInformation($"Launcher region refreshed with {FrequentApps.Count} items");
    }
}

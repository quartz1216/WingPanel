using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WingPanel.App.Services;
using WingPanel.App.Views.Regions;
using WingPanel.Core.Models.Settings;

namespace WingPanel.App.ViewModels;

public sealed class PanelWindowViewModel
{
    private readonly ServiceBootstrapper _services;
    private readonly Dictionary<string, RegionHostViewModel> _regionMap;

    public PanelWindowViewModel(ServiceBootstrapper services)
    {
        _services = services;
        _regionMap = CreateRegionMap();
        Regions = new ObservableCollection<RegionHostViewModel>();

        foreach (var region in _regionMap.Values)
        {
            region.Initialize(_services);
        }

        ApplyLayout(_services.Settings.Current.Layout.OrderedRegions);
        _services.Settings.SettingsChanged += OnSettingsChanged;
        _services.Hotkeys.Reset();
        _services.Hotkeys.TryRegister("PanelToggle", _services.Settings.Current.ToggleHotkey);
    }

    public ObservableCollection<RegionHostViewModel> Regions { get; }

    private Dictionary<string, RegionHostViewModel> CreateRegionMap()
    {
        return new Dictionary<string, RegionHostViewModel>
        {
            [RegionNames.Launcher] = new RegionHostViewModel(RegionNames.Launcher, "Launcher", new LauncherRegion()),
            [RegionNames.TaskSwitcher] = new RegionHostViewModel(RegionNames.TaskSwitcher, "Tasks", new TaskSwitcherRegion()),
            [RegionNames.Volume] = new RegionHostViewModel(RegionNames.Volume, "Volume", new VolumeRegion()),
            [RegionNames.Notifications] = new RegionHostViewModel(RegionNames.Notifications, "Notifications", new NotificationsRegion())
        };
    }

    private void ApplyLayout(IEnumerable<string> order)
    {
        var seen = new HashSet<string>();
        Regions.Clear();
        foreach (var id in order)
        {
            if (_regionMap.TryGetValue(id, out var region) && seen.Add(id))
            {
                Regions.Add(region);
            }
        }

        foreach (var kvp in _regionMap)
        {
            if (seen.Add(kvp.Key))
            {
                Regions.Add(kvp.Value);
            }
        }
    }

    private async void OnSettingsChanged(object? sender, WingPanel.Core.Models.Settings.WingPanelSettings settings)
    {
        ApplyLayout(settings.Layout.OrderedRegions);
        await CommitRegionOrderAsync().ConfigureAwait(false);
    }

    public async Task CommitRegionOrderAsync()
    {
        _services.Settings.Current.Layout.OrderedRegions = Regions.Select(r => r.Id).ToList();
        await _services.Settings.SaveAsync().ConfigureAwait(false);
    }
}

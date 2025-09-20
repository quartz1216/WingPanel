using Microsoft.UI.Xaml.Controls;
using WingPanel.App.Services;
using WingPanel.App.Views.Regions;

namespace WingPanel.App.ViewModels;

public sealed class RegionHostViewModel
{
    public RegionHostViewModel(string id, string displayName, UserControl view)
    {
        Id = id;
        DisplayName = displayName;
        View = view;
    }

    public string Id { get; }

    public string DisplayName { get; }

    public UserControl View { get; }

    public void Initialize(ServiceBootstrapper services)
    {
        if (View is IRegionControl regionControl)
        {
            regionControl.Initialize(services);
        }
    }
}

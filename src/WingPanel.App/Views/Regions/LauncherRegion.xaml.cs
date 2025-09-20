using Microsoft.UI.Xaml.Controls;
using WingPanel.App.Services;
using WingPanel.App.ViewModels.Regions;

namespace WingPanel.App.Views.Regions;

public sealed partial class LauncherRegion : UserControl, IRegionControl
{
    private ServiceBootstrapper? _services;

    public LauncherRegion()
    {
        InitializeComponent();
    }

    public LauncherRegionViewModel ViewModel { get; private set; } = null!;

    public void Initialize(ServiceBootstrapper services)
    {
        _services = services;
        ViewModel = new LauncherRegionViewModel(services.Settings, services.Logger);
        DataContext = ViewModel;
        Bindings.Update();
    }

    private void LauncherList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (_services is null)
        {
            return;
        }

        if (e.ClickedItem is string app)
        {
            _services.Logger.LogInformation($"Launcher invoked {app}");
        }
    }
}

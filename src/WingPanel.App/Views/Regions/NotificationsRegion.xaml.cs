using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WingPanel.App.Services;
using WingPanel.App.ViewModels.Regions;

namespace WingPanel.App.Views.Regions;

public sealed partial class NotificationsRegion : UserControl, IRegionControl
{
    public NotificationsRegion()
    {
        InitializeComponent();
    }

    public NotificationsRegionViewModel ViewModel { get; private set; } = null!;

    public void Initialize(ServiceBootstrapper services)
    {
        ViewModel = new NotificationsRegionViewModel(services.ActionCenter, services.Settings);
        DataContext = ViewModel;
        Bindings.Update();
    }

    private void OnToggleActionCenter(object sender, RoutedEventArgs e)
    {
        ViewModel?.ToggleActionCenter();
    }
}

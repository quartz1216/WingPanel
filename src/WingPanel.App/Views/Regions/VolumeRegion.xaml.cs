using Microsoft.UI.Xaml.Controls;
using WingPanel.App.Services;
using WingPanel.App.ViewModels.Regions;

namespace WingPanel.App.Views.Regions;

public sealed partial class VolumeRegion : UserControl, IRegionControl
{
    public VolumeRegion()
    {
        InitializeComponent();
    }

    public VolumeRegionViewModel ViewModel { get; private set; } = null!;

    public void Initialize(ServiceBootstrapper services)
    {
        ViewModel = new VolumeRegionViewModel(services.Audio);
        DataContext = ViewModel;
        Bindings.Update();
    }
}

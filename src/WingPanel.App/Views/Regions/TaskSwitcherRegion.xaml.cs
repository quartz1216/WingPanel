using Microsoft.UI.Xaml.Controls;
using WingPanel.App.Services;
using WingPanel.App.ViewModels.Regions;

namespace WingPanel.App.Views.Regions;

public sealed partial class TaskSwitcherRegion : UserControl, IRegionControl
{
    public TaskSwitcherRegion()
    {
        InitializeComponent();
    }

    public TaskSwitcherRegionViewModel ViewModel { get; private set; } = null!;

    public void Initialize(ServiceBootstrapper services)
    {
        ViewModel = new TaskSwitcherRegionViewModel(services.Windows);
        DataContext = ViewModel;
        Bindings.Update();
    }

    private void OnPrevClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel?.MoveSelection(-1);
    }

    private void OnNextClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel?.MoveSelection(1);
    }
}

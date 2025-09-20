using Microsoft.UI.Xaml;
using WingPanel.App.Services;

namespace WingPanel.App;

public partial class App : Application
{
    private readonly ServiceBootstrapper _services;
    private PanelWindow? _window;

    public App()
    {
        InitializeComponent();
        _services = new ServiceBootstrapper();
        UnhandledException += OnUnhandledException;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window ??= new PanelWindow(_services);
        _window.Activate();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _services.Logger.LogError("Unhandled exception", e.Exception);
        e.Handled = true;
    }
}

using System.Collections.ObjectModel;
using WingPanel.Core.Models.Settings;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.App.ViewModels.Regions;

public sealed class NotificationsRegionViewModel : ObservableObject
{
    private readonly IActionCenterService _actionCenter;
    private readonly ISettingsService _settings;
    private bool _isActionCenterVisible;
    private bool _doNotDisturb;

    public NotificationsRegionViewModel(IActionCenterService actionCenter, ISettingsService settings)
    {
        _actionCenter = actionCenter;
        _settings = settings;
        Notifications = new ObservableCollection<string>();
        _doNotDisturb = settings.Current.Notifications.EnableDoNotDisturb;
        _actionCenter.VisibilityChanged += OnVisibilityChanged;
        _settings.SettingsChanged += OnSettingsChanged;
        GeneratePlaceholder();
    }

    public ObservableCollection<string> Notifications { get; }

    public bool IsActionCenterVisible
    {
        get => _isActionCenterVisible;
        set => SetProperty(ref _isActionCenterVisible, value);
    }

    public bool DoNotDisturb
    {
        get => _doNotDisturb;
        set
        {
            if (SetProperty(ref _doNotDisturb, value))
            {
                _settings.Current.Notifications.EnableDoNotDisturb = value;
            }
        }
    }

    public void ToggleActionCenter() => _actionCenter.Toggle();

    private void OnVisibilityChanged(object? sender, bool visible)
    {
        IsActionCenterVisible = visible;
    }

    private void OnSettingsChanged(object? sender, WingPanelSettings settings)
    {
        DoNotDisturb = settings.Notifications.EnableDoNotDisturb;
    }

    private void GeneratePlaceholder()
    {
        Notifications.Clear();
        Notifications.Add("No notifications");
    }
}

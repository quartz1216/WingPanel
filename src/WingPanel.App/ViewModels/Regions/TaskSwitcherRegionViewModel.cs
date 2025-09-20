using System.Collections.Generic;
using System.Collections.ObjectModel;
using WingPanel.Core.Models;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.App.ViewModels.Regions;

public sealed class TaskSwitcherRegionViewModel : ObservableObject
{
    private readonly IWindowEnumService _windowService;
    private WindowInfo? _selected;

    public TaskSwitcherRegionViewModel(IWindowEnumService windowService)
    {
        _windowService = windowService;
        Windows = new ObservableCollection<WindowInfo>(windowService.CurrentWindows);
        if (Windows.Count > 0)
        {
            SelectedWindow = Windows[0];
        }

        _windowService.WindowsChanged += OnWindowsChanged;
    }

    public ObservableCollection<WindowInfo> Windows { get; }

    public WindowInfo? SelectedWindow
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public void MoveSelection(int delta)
    {
        if (Windows.Count == 0)
        {
            return;
        }

        var currentIndex = SelectedWindow is null ? 0 : Windows.IndexOf(SelectedWindow);
        var targetIndex = (currentIndex + delta + Windows.Count) % Windows.Count;
        SelectedWindow = Windows[targetIndex];
        _windowService.NotifyActivated(SelectedWindow);
    }

    private void OnWindowsChanged(object? sender, IReadOnlyList<WindowInfo> windows)
    {
        Windows.Clear();
        foreach (var window in windows)
        {
            Windows.Add(window);
        }

        if (Windows.Count > 0)
        {
            SelectedWindow = Windows[0];
        }
        else
        {
            SelectedWindow = null;
        }
    }
}

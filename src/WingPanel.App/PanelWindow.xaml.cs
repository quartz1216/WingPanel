using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WingPanel.App.Services;
using WingPanel.App.ViewModels;
using WingPanel.Core.Models;
using Windows.Graphics;
using Windows.System;

namespace WingPanel.App;

public sealed partial class PanelWindow : Window
{
    private readonly ServiceBootstrapper _services;
    private AppWindow? _appWindow;

    public PanelWindow(ServiceBootstrapper services)
    {
        InitializeComponent();
        _services = services;
        ViewModel = new PanelWindowViewModel(services);
        DataContext = ViewModel;

        RegionList.DragItemsCompleted += OnDragItemsCompleted;
        RegionList.PreviewKeyDown += OnRegionListPreviewKeyDown;
        _services.DisplayBounds.DisplayChanged += OnDisplayChanged;

        UpdateWindowBounds(_services.DisplayBounds.GetPrimaryDisplay());
    }

    public PanelWindowViewModel ViewModel { get; }

    private async void OnDragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        await ViewModel.CommitRegionOrderAsync().ConfigureAwait(false);
    }

    private void OnDisplayChanged(object? sender, DisplayBounds bounds)
    {
        DispatcherQueue.TryEnqueue(() => UpdateWindowBounds(bounds));
    }

    private void UpdateWindowBounds(DisplayBounds bounds)
    {
        EnsureAppWindow();
        if (_appWindow is null)
        {
            return;
        }

        var width = Math.Min(bounds.Width, 1200);
        var height = Math.Min(bounds.Height / 2, 480);
        var x = bounds.X + (bounds.Width - width) / 2;
        var y = bounds.Y + 20;
        _appWindow.MoveAndResize(new RectInt32(x, y, width, height));
    }

    private void EnsureAppWindow()
    {
        if (_appWindow is not null)
        {
            return;
        }

        var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = WinRT.Interop.Win32Interop.GetWindowIdFromWindow(handle);
        _appWindow = AppWindow.GetFromWindowId(windowId);
    }

    private void OnRegionListPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Left or VirtualKey.Right)
        {
            var list = (ListView)sender;
            var direction = e.Key == VirtualKey.Left ? -1 : 1;
            if (list.SelectedIndex < 0 && list.Items.Count > 0)
            {
                list.SelectedIndex = 0;
                return;
            }

            var target = Math.Clamp(list.SelectedIndex + direction, 0, list.Items.Count - 1);
            list.SelectedIndex = target;
            list.ScrollIntoView(list.SelectedItem, ScrollIntoViewAlignment.Leading);
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Escape)
        {
            Close();
            e.Handled = true;
        }
    }
}

using System;

namespace WingPanel.Core.Services.Interfaces;

public interface IActionCenterService
{
    event EventHandler<bool>? VisibilityChanged;

    bool IsVisible { get; }

    void Show();

    void Hide();

    void Toggle();
}

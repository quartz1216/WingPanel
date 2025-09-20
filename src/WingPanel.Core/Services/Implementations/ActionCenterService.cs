using System;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public sealed class ActionCenterService : IActionCenterService
{
    private bool _isVisible;

    public event EventHandler<bool>? VisibilityChanged;

    public bool IsVisible => _isVisible;

    public void Show()
    {
        if (!_isVisible)
        {
            _isVisible = true;
            VisibilityChanged?.Invoke(this, true);
        }
    }

    public void Hide()
    {
        if (_isVisible)
        {
            _isVisible = false;
            VisibilityChanged?.Invoke(this, false);
        }
    }

    public void Toggle()
    {
        if (_isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}

using System;
using WingPanel.Core.Models;

namespace WingPanel.Core.Services.Interfaces;

public interface IDisplayBoundsService
{
    event EventHandler<DisplayBounds>? DisplayChanged;

    DisplayBounds GetPrimaryDisplay();
}

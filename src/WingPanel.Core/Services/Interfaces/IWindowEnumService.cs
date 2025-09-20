using System;
using System.Collections.Generic;
using WingPanel.Core.Models;

namespace WingPanel.Core.Services.Interfaces;

public interface IWindowEnumService
{
    event EventHandler<IReadOnlyList<WindowInfo>>? WindowsChanged;

    IReadOnlyList<WindowInfo> CurrentWindows { get; }

    void Refresh();

    void NotifyActivated(WindowInfo window);
}

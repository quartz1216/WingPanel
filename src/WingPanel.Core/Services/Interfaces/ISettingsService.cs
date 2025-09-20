using System;
using System.Threading;
using System.Threading.Tasks;
using WingPanel.Core.Models.Settings;

namespace WingPanel.Core.Services.Interfaces;

public interface ISettingsService
{
    event EventHandler<WingPanelSettings>? SettingsChanged;

    WingPanelSettings Current { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);

    Task ReloadAsync(CancellationToken cancellationToken = default);
}

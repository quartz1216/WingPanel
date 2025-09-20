using WingPanel.Core.Logging;
using WingPanel.Core.Services.Implementations;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.App.Services;

public sealed class ServiceBootstrapper
{
    public ServiceBootstrapper()
    {
        Logger = new LogService();
        Settings = new SettingsService(Logger);
        Hotkeys = new HotkeyService(logger: Logger);
        Cursor = new CursorService();
        DisplayBounds = new DisplayBoundsService();
        Audio = new AudioService();
        Windows = new WindowEnumService();
        ActionCenter = new ActionCenterService();
    }

    public ILogService Logger { get; }

    public ISettingsService Settings { get; }

    public IHotkeyService Hotkeys { get; }

    public ICursorService Cursor { get; }

    public IDisplayBoundsService DisplayBounds { get; }

    public IAudioService Audio { get; }

    public IWindowEnumService Windows { get; }

    public IActionCenterService ActionCenter { get; }
}

using System;
using System.IO;
using System.Threading.Tasks;
using WingPanel.Core.Models.Settings;
using WingPanel.Core.Services.Implementations;
using WingPanel.Core.Services.Interfaces;
using Xunit;

namespace WingPanel.Tests;

public sealed class SettingsServiceTests : IDisposable
{
    private readonly string _baseDirectory;

    public SettingsServiceTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_baseDirectory);
    }

    [Fact]
    public async Task SaveAndReload_PreservesChanges()
    {
        var service = new SettingsService(new TestLogService(), _baseDirectory);
        service.Current.Launcher.MaxFrequentApps = 12;
        await service.SaveAsync();

        service.Current.Launcher.MaxFrequentApps = 1;
        await service.ReloadAsync();

        Assert.Equal(12, service.Current.Launcher.MaxFrequentApps);
    }

    [Fact]
    public void Load_MigratesOlderVersion()
    {
        var json = "{\"version\":1,\"launcher\":{\"showFrequentApps\":true,\"maxFrequentApps\":4}}";
        File.WriteAllText(Path.Combine(_baseDirectory, "Settings.json"), json);

        var service = new SettingsService(new TestLogService(), _baseDirectory);

        Assert.Equal(WingPanelSettings.CurrentVersion, service.Current.Version);
        Assert.NotEmpty(service.Current.Layout.OrderedRegions);
    }

    public void Dispose()
    {
        if (Directory.Exists(_baseDirectory))
        {
            Directory.Delete(_baseDirectory, recursive: true);
        }
    }

    private sealed class TestLogService : ILogService
    {
        public void Log(Microsoft.Extensions.Logging.LogLevel level, string message, Exception? exception = null)
        {
        }
    }
}

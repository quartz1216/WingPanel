using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Logging;

public sealed class LogService : ILogService, IDisposable
{
    private readonly object _sync = new();
    private readonly string _logDirectory;
    private StreamWriter? _writer;
    private DateTime _currentDate;

    public LogService(string? baseDirectory = null)
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _logDirectory = baseDirectory ?? Path.Combine(localAppData, "WingPanel", "Logs");
        Directory.CreateDirectory(_logDirectory);
        EnsureWriter();
    }

    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        EnsureWriter();
        var line = string.Format(CultureInfo.InvariantCulture, "{0:o} [{1}] {2}", DateTime.UtcNow, level, message);
        lock (_sync)
        {
            _writer?.WriteLine(line);
            if (exception is not null)
            {
                _writer?.WriteLine(exception);
            }
        }
    }

    private void EnsureWriter()
    {
        var today = DateTime.UtcNow.Date;
        lock (_sync)
        {
            if (_writer is not null && today == _currentDate)
            {
                return;
            }

            _writer?.Dispose();
            _currentDate = today;
            var file = Path.Combine(_logDirectory, $"wingpanel-{today:yyyyMMdd}.log");
            _writer = new StreamWriter(new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                AutoFlush = true
            };
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _writer?.Dispose();
            _writer = null;
        }
    }
}

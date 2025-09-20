using System;
using Microsoft.Extensions.Logging;

namespace WingPanel.Core.Services.Interfaces;

public interface ILogService
{
    void Log(LogLevel level, string message, Exception? exception = null);

    void LogInformation(string message) => Log(LogLevel.Information, message);

    void LogWarning(string message) => Log(LogLevel.Warning, message);

    void LogError(string message, Exception? exception = null) => Log(LogLevel.Error, message, exception);
}

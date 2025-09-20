using System;

namespace WingPanel.Core.Services.Interfaces;

public interface IAudioService
{
    event EventHandler<double>? VolumeChanged;

    event EventHandler<bool>? MutedChanged;

    double Volume { get; }

    bool IsMuted { get; }

    void SetVolume(double value);

    void ToggleMute();
}

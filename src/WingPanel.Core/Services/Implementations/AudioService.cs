using System;
using WingPanel.Core.Services.Interfaces;

namespace WingPanel.Core.Services.Implementations;

public sealed class AudioService : IAudioService
{
    private double _volume = 0.5;
    private bool _isMuted;

    public event EventHandler<double>? VolumeChanged;

    public event EventHandler<bool>? MutedChanged;

    public double Volume => _volume;

    public bool IsMuted => _isMuted;

    public void SetVolume(double value)
    {
        var clamped = Math.Clamp(value, 0d, 1d);
        if (Math.Abs(clamped - _volume) > 0.0001)
        {
            _volume = clamped;
            VolumeChanged?.Invoke(this, _volume);
        }

        if (_volume > 0 && _isMuted)
        {
            _isMuted = false;
            MutedChanged?.Invoke(this, _isMuted);
        }
    }

    public void ToggleMute()
    {
        _isMuted = !_isMuted;
        MutedChanged?.Invoke(this, _isMuted);
    }
}

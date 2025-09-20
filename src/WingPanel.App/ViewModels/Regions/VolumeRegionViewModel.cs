using WingPanel.Core.Services.Interfaces;

namespace WingPanel.App.ViewModels.Regions;

public sealed class VolumeRegionViewModel : ObservableObject
{
    private readonly IAudioService _audioService;
    private double _volume;
    private bool _isMuted;
    private bool _updating;

    public VolumeRegionViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _volume = _audioService.Volume * 100d;
        _isMuted = _audioService.IsMuted;
        _audioService.VolumeChanged += OnVolumeChanged;
        _audioService.MutedChanged += OnMutedChanged;
    }

    public double Volume
    {
        get => _volume;
        set
        {
            if (SetProperty(ref _volume, value) && !_updating)
            {
                _audioService.SetVolume(value / 100d);
            }
        }
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (SetProperty(ref _isMuted, value) && !_updating)
            {
                if (value != _audioService.IsMuted)
                {
                    _audioService.ToggleMute();
                }
            }
        }
    }

    private void OnVolumeChanged(object? sender, double volume)
    {
        _updating = true;
        Volume = volume * 100d;
        _updating = false;
    }

    private void OnMutedChanged(object? sender, bool isMuted)
    {
        _updating = true;
        IsMuted = isMuted;
        _updating = false;
    }
}

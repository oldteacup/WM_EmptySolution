using NAudio.Wave;
using NAudioService.Shared.Enums;
using System;

namespace NAudioService.Shared.Models
{
    internal interface IAudioObjectProcess
    {
        IWaveProvider AudioFade(ISampleProvider provider, int milliseconds, (TimeSpan, TimeSpan) timeInterval, FadeState state);

        bool AudioCut(string destinationPath, (TimeSpan, TimeSpan) timeInterval, Action<string> msgAction);

        void AudioSpeed(float speed);

        void AudioVolume(float volume);
    }
}

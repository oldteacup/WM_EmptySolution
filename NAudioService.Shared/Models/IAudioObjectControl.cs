using NAudioService.Shared.Enums;
using System;

namespace NAudioService.Shared.Models
{
    internal interface IAudioObjectControl
    {
        public event EventHandler<PlayStatus> PlayStatusChangedEventHandler;

        void Play();

        void Play(TimeSpan startTimeSpan);

        void Reposition(TimeSpan startTimeSpan);

        void Stop();

        void Pause();

        void RePlay();
    }
}

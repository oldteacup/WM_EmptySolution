using AudioClips.Desktop.SoundTouch;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudioService.Shared.Enums;
using NAudioService.Shared.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using WaveFormRendererLib;

namespace NAudioService.Shared.Models
{
    public class AudioObject : IAudioObjectIO, IAudioObjectControl, IAudioObjectProcess, IAudioVisualization, INotifyPropertyChanged
    {
        private bool _isLoop = false;
        private PlayStatus _status = PlayStatus.Stop;
        private readonly object _waveOutEventLocker = new object();
        private string _sourcePath = string.Empty;
        private VarispeedSampleProvider _speedControl;
        private WaveOutEvent _waveOutEvent = new();
        private Dictionary<ProviderMode, ISampleProvider> _providers = new Dictionary<ProviderMode, ISampleProvider>();
        public AudioFileReader AudioReader { get; private set; }
        private WaveStream _waveStream;
        public Color PenColor = Color.White, BackgroundColor = Color.Black;
        public event EventHandler<bool> OpenFinished;
        public event EventHandler<PlayStatus> PlayStatusChangedEventHandler;
        public event PropertyChangedEventHandler PropertyChanged;
        public string FileName { get => AudioReader?.FileName; }
        public TimeSpan CurrentTime { get => AudioReader?.CurrentTime ?? TimeSpan.Zero; set => Reposition(value); }
        public TimeSpan TotalTime { get => AudioReader?.TotalTime ?? TimeSpan.Zero; }

        public AudioObject(string sourcePath)
        {
            Open(sourcePath);
            OnPropertyChanged(nameof(FileName));
        }

        ~AudioObject()
        {
            _waveOutEvent?.Dispose();
            AudioReader?.DisposeAsync();
        }

        public PlayStatus Status
        {
            get => _status;
            set
            {
                OnPropertyChanged(ref _status, value);
                PlayStatusChangedEventHandler?.Invoke(this, value);
            }
        }

        private (TimeSpan, TimeSpan) _selectionInterval;
        public (TimeSpan, TimeSpan) SelectionInterval
        {
            get
            {
                return _selectionInterval;
            }
            set
            {
                OnPropertyChanged(ref _selectionInterval, value);
            }
        }

        public bool IsLoop
        {
            get => _isLoop; set => OnPropertyChanged(ref _isLoop, value);
        }

        public void Open(string sourcePath)
        {
            try
            {
                _sourcePath = sourcePath;
                AudioReader = new AudioFileReader(sourcePath);
                _waveOutEvent.Init(AudioReader);
                _speedControl = new VarispeedSampleProvider(AudioReader, 100, new SoundTouchProfile(true, false));
                OpenFinished?.Invoke(null, true);
            }
            catch (Exception e)
            {
                OpenFinished?.Invoke(null, false);
            }
        }

        public void Save(string destinationPath)
        {
        }

        public void Pause()
        {
            lock (_waveOutEventLocker)
            {
                _waveOutEvent.Pause();
                Status = PlayStatus.Pause;
            }
        }

        public void Play()
        {
            lock (_waveOutEventLocker)
            {
                _waveOutEvent.Play();
                Status = PlayStatus.Play;
            }
        }

        public void Reposition(TimeSpan startTimeSpan)
        {
            lock (_waveOutEventLocker)
            {
                AudioReader.CurrentTime = startTimeSpan;
                _speedControl?.Reposition();
            }
        }

        public void Play(TimeSpan startTimeSpan)
        {
            lock (_waveOutEventLocker)
            {
                AudioReader.CurrentTime = startTimeSpan;
                _speedControl.Reposition();
                Status = PlayStatus.Play;
            }
        }

        public void RePlay()
        {
            lock (_waveOutEventLocker)
            {
                _waveOutEvent.Stop();
                _waveOutEvent.Play();
                Status = PlayStatus.Play;
            }
        }

        public void Stop()
        {
            lock (_waveOutEventLocker)
            {
                _waveOutEvent.Stop();
                Status = PlayStatus.Stop;
            }
        }

        public void AudioSpeed(float speed)
        {
            throw new NotImplementedException();
        }

        public void AudioVolume(float volume)
        {
            _waveOutEvent.Volume = volume;
        }

        public IWaveProvider AudioFade(ISampleProvider provider, int milliseconds, (TimeSpan, TimeSpan) timeInterval, FadeState state)
        {
            int bytesPerMillisecond = AudioReader.WaveFormat.AverageBytesPerSecond / 1000;

            int startPos = (int)timeInterval.Item1.TotalSeconds * AudioReader.WaveFormat.AverageBytesPerSecond;
            startPos -= startPos % AudioReader.WaveFormat.BlockAlign;

            int endPos = (int)timeInterval.Item2.TotalSeconds * AudioReader.WaveFormat.AverageBytesPerSecond;
            endPos -= endPos % AudioReader.WaveFormat.BlockAlign;
            FadeInOutProvider fade = new FadeInOutProvider(provider);
            switch (state)
            {
                case FadeState.Silence:
                    break;
                case FadeState.FadingIn:
                    fade.BeginFadeIn(milliseconds, startPos);
                    break;
                case FadeState.FullVolume:
                    break;
                case FadeState.FadingOut:
                    fade.BeginFadeOut(milliseconds, endPos);
                    break;
            }
            return new SampleToWaveProvider(fade);// fade.ToWaveProvider();
        }

        public bool AudioCut(string destinationPath, (TimeSpan, TimeSpan) timeInterval, Action<string> msgAction)
        {
            lock (_waveOutEventLocker)
            {
                var fadeInProvider = AudioFade(AudioReader, 20000, timeInterval, FadeState.FadingIn);
                var tmpProvider = new WaveToSampleProvider(fadeInProvider);
                var fadeInProvider2 = AudioFade(tmpProvider, 20000, timeInterval, FadeState.FadingOut);
                WaveFileWriter.CreateWaveFile(destinationPath, fadeInProvider2);
            }
            return true;
            lock (_waveOutEventLocker)
            {
                double Count = 0;
                //using WaveFileReader reader = new WaveFileReader(_sourcePath);
                try
                {
                    //msgAction?.Invoke(Application.Current?.FindResource("File_Loading_Text")?.ToString());
                    using var writer = new WaveFileWriter(destinationPath, AudioReader.WaveFormat);
                    int bytesPerMillisecond = AudioReader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)timeInterval.Item1.TotalMilliseconds * bytesPerMillisecond;
                    startPos -= startPos % AudioReader.WaveFormat.BlockAlign;

                    int endPos = (int)timeInterval.Item2.TotalMilliseconds * bytesPerMillisecond;
                    endPos -= endPos % AudioReader.WaveFormat.BlockAlign;
                    //int endPos = (int)_audioFile.Length - endBytes;

                    AudioReader.Position = startPos;
                    byte[] buffer = new byte[1024];
                    Count = endPos - startPos;
                    while (AudioReader.Position < endPos)
                    {
                        int bytesRequired = (int)(endPos - AudioReader.Position);
                        if (bytesRequired > 0)
                        {
                            int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                            int bytesRead = AudioReader.Read(buffer, 0, bytesToRead);
                            if (bytesRead > 0)
                            {
                                writer.Write(buffer, 0, bytesRead);
                            }
                        }
                        //msgAction?.Invoke((1 - (bytesRequired / Count)).ToString("0.00") + "%");
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<Image> AudioVisualAsync()
        {
            return await Task.Run(() =>
            {
                return AudioVisual();
            });
        }

        private Image _audioImage = null;
        public Image AudioVisual()
        {
            lock (_waveOutEventLocker)
            {
                if (_audioImage == null)
                {
                    if (string.IsNullOrWhiteSpace(_sourcePath) || !File.Exists(_sourcePath))
                    {
                        return null;
                    }
                    //Settings
                    var pen = new Pen(PenColor);
                    var settings = new StandardWaveFormRendererSettings()
                    {
                        Width = 450,
                        TopHeight = 20,
                        BottomHeight = 20,
                        TopPeakPen = pen,
                        BottomPeakPen = pen,
                        BackgroundColor = BackgroundColor
                    };
                    try
                    {
                        _audioImage = AudioReader.Render(settings);
                    }
                    catch
                    {
                    }
                }
                return _audioImage;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
        private void OnPropertyChanged<T>(ref T target, T val, [CallerMemberName] string propertyName = null)
        {
            target = val;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
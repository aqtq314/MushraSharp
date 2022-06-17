using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonKit
{
    public class AudioPlaybackEngine : DependencyObject, ISampleProvider
    {
        public const int Fs = 44100;
        public const int Channels = 1;

        public static WaveFormat PlaybackWaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(Fs, Channels);

        public static AudioPlaybackEngine Instance { get; } = new AudioPlaybackEngine();

        AudioPlayer? _activePlayer;
        bool _isPlaying;
        bool _isSwitchingPlayer;

        public DirectSoundOut WaveOut { get; init; }
        public WaveFormat WaveFormat => PlaybackWaveFormat;

        public AudioPlayer? ActivePlayer
        {
            get => _activePlayer;
            set
            {
                if (_activePlayer == value) return;

                var isPlaying = IsPlaying;
                _isSwitchingPlayer = true;
                try
                {
                    IsPlaying = false;
                    _activePlayer = value;
                    IsPlaying = isPlaying;
                }
                finally
                {
                    _isSwitchingPlayer = false;
                }
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value) return;

                _isPlaying = value;
                if (!_isSwitchingPlayer)
                {
                    if (_isPlaying)
                        WaveOut.Play();
                    else
                        WaveOut.Stop();
                }

                var activePlayer = ActivePlayer;
                if (activePlayer != null)
                    activePlayer.OnEngineIsPlayingChanged(_isPlaying);
            }
        }

        private AudioPlaybackEngine()
        {
            int latency = 80;
            WaveOut = new DirectSoundOut(latency);
            WaveOut.Init(this);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var activePlayer = ActivePlayer;

            Array.Clear(buffer, offset, count * sizeof(float));

            var samplesRead = 0;
            if (activePlayer != null)
                samplesRead = activePlayer.EngineReadSamples(buffer, offset, count);

            if (samplesRead == 0)
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsPlaying = false;
                }));

            return count;
        }
    }
}

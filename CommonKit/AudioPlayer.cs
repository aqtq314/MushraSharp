using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonKit
{
    public class AudioPlayer : Control
    {
        static AudioPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AudioPlayer), new FrameworkPropertyMetadata(typeof(AudioPlayer)));
        }

        public static DependencyProperty AudioSourceProperty { get; } =
            DependencyProperty.Register(nameof(AudioSource), typeof(AudioSource), typeof(AudioPlayer),
                new FrameworkPropertyMetadata(null,
                    (d, e) => ((AudioPlayer)d).OnAudioSourceChanged((AudioSource?)e.OldValue, (AudioSource?)e.NewValue)));

        public static DependencyProperty LoopProperty { get; } =
            DependencyProperty.Register(nameof(Loop), typeof(bool), typeof(AudioPlayer),
                new FrameworkPropertyMetadata(true,
                    (d, e) => ((AudioPlayer)d).OnLoopChanged((bool)e.OldValue, (bool)e.NewValue)));

        public static DependencyProperty IsPlayingProperty { get; } =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(AudioPlayer),
                new FrameworkPropertyMetadata(false,
                    (d, e) => ((AudioPlayer)d).OnIsPlayingChanged((bool)e.OldValue, (bool)e.NewValue)));

        public static DependencyProperty PlaybackPositionProperty { get; } =
            DependencyProperty.Register(nameof(PlaybackPosition), typeof(double), typeof(AudioPlayer),
                new FrameworkPropertyMetadata(0.0,
                    (d, e) => ((AudioPlayer)d).OnPlaybackPositionChanged((double)e.OldValue, (double)e.NewValue)));

        AudioSource? _playbackAudioSource;
        bool _playbackLoop = true;
        bool _playbackPositionSyncking;
        long _playbackPositionRefTicks;
        int _playbackPosition;

        public AudioSource? AudioSource
        {
            get => (AudioSource?)GetValue(AudioSourceProperty);
            set => SetValue(AudioSourceProperty, value);
        }

        public bool Loop
        {
            get => (bool)GetValue(LoopProperty);
            set => SetValue(LoopProperty, value);
        }

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public double PlaybackPosition
        {
            get => (double)GetValue(PlaybackPositionProperty);
            set => SetValue(PlaybackPositionProperty, value);
        }

        public AudioPlayer() { }

        internal int EngineReadSamples(float[] buffer, int offset, int count)
        {
            var audioSource = _playbackAudioSource;
            if (audioSource == null) return 0;

            var playbackPosition = _playbackPosition;
            var audioSamples = audioSource.Samples;
            var loop = _playbackLoop;
            var copyLength = Math.Max(0, Math.Min(count, audioSamples.Length - playbackPosition));
            Buffer.BlockCopy(audioSamples, playbackPosition * sizeof(float), buffer, offset * sizeof(float), copyLength * sizeof(float));
            playbackPosition += copyLength;

            if (playbackPosition >= audioSamples.Length)
            {
                playbackPosition = 0;
                if (!loop)
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        IsPlaying = false;
                        _playbackPosition = 0;
                    }));
            }

            _playbackPositionRefTicks = Stopwatch.GetTimestamp();
            _playbackPosition = playbackPosition;
            return copyLength;
        }

        internal void OnEngineIsPlayingChanged(bool engineIsPlaying)
        {
            IsPlaying = engineIsPlaying;

            if (engineIsPlaying)
            {
                CompositionTarget.Rendering += OnCompositionTargetRendering;
            }
            else
            {
                CompositionTarget.Rendering -= OnCompositionTargetRendering;
                SyncPlaybackPosition();
            }
        }

        private void OnAudioSourceChanged(AudioSource? oldValue, AudioSource? newValue)
        {
            _playbackAudioSource = newValue;
            PlaybackPosition = 0;
        }

        private void OnLoopChanged(bool oldValue, bool newValue)
        {
            _playbackLoop = newValue;
        }

        private void OnIsPlayingChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                AudioPlaybackEngine.Instance.ActivePlayer = this;
                _playbackPositionRefTicks = Stopwatch.GetTimestamp();
            }

            AudioPlaybackEngine.Instance.IsPlaying = newValue;
        }

        private void SyncPlaybackPosition(double? setToValue = null)
        {
            if (_playbackPositionSyncking) return;

            _playbackPositionSyncking = true;
            try
            {
                var audioSource = AudioSource;
                if (audioSource == null) return;

                if (setToValue.HasValue)
                {
                    _playbackPositionRefTicks = Stopwatch.GetTimestamp();
                    _playbackPosition = (int)(setToValue.Value * AudioPlaybackEngine.Fs);
                }
                else
                {
                    PlaybackPosition = (double)_playbackPosition / AudioPlaybackEngine.Fs +
                        (!IsPlaying ? 0 : (double)(Stopwatch.GetTimestamp() - _playbackPositionRefTicks) / TimeSpan.TicksPerSecond);
                }
            }
            finally
            {
                _playbackPositionSyncking = false;
            }
        }

        private void OnCompositionTargetRendering(object? sender, EventArgs e)
        {
            SyncPlaybackPosition();
        }

        private void OnPlaybackPositionChanged(double oldValue, double newValue)
        {
            SyncPlaybackPosition(newValue);
        }
    }
}

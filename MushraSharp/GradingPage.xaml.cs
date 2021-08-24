﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MushraSharp
{
    public partial class GradingPage : PageWithNext
    {
        public GradePageVM GradePageVM => (GradePageVM)DataContext;
        public string PageTitle => $"第 {PageIndex + 1} 组";

        bool _progressSync;

        public GradingPage(MasterVM masterVM, int pageIndex) : base(masterVM, pageIndex)
        {
            InitializeComponent();
            DataContext = masterVM.GradePages[pageIndex];
            mediaElement.Source = new Uri(GradePageVM.RefAudioPath);

            CompositionTarget.Rendering += (sender, e) =>
            {
                if (!_progressSync && playButton.IsChecked == true)
                {
                    _progressSync = true;
                    timelineSlider.Value = mediaElement.Position.TotalSeconds;
                    _progressSync = false;
                }
            };
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            playButton.IsChecked = false;
            mediaElement.Stop();
            timelineSlider.Value = 0;
        }

        private void OnPlayButtonChecked(object sender, RoutedEventArgs e) => mediaElement.Play();
        private void OnPlayButtonUnchecked(object sender, RoutedEventArgs e) => mediaElement.Pause();

        private void OnNextGradingPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM.GradeItems.Any(gradeItem => gradeItem.Grade == 100))
                OnNextPageButtonClicked(sender, e);

            else
                MessageBox.Show("至少一项评分须为 100。", "评分检查", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnRefAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM == null) return;
            mediaElement.Source = new Uri(GradePageVM.RefAudioPath);
        }

        private void OnAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            var gradeItemVM = (GradeItemVM)((RadioButton)sender).DataContext;
            mediaElement.Source = new Uri(gradeItemVM.AudioPath);
        }

        private void OnMediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void OnTimelineSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_progressSync)
            {
                _progressSync = true;
                mediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
                _progressSync = false;
            }
        }

        private void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            if (loopButton.IsChecked == true)
                mediaElement.Position = TimeSpan.Zero;

            else
            {
                playButton.IsChecked = false;
                mediaElement.Stop();
                timelineSlider.Value = 0;
            }
        }
    }
}
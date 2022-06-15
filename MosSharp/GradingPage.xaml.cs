using System;
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

namespace MosSharp
{
    public partial class GradingPage : PageWithNext
    {
        public GradePageVM GradePageVM => (GradePageVM)DataContext;
        public int PageCount { get; init; }
        public int AudioItemCount { get; init; }

        //bool _progressSync;
        DateTime _loadedTime;

        public GradingPage(MasterVM masterVM, int pageIndex) : base(masterVM, pageIndex)
        {
            InitializeComponent();
            PageCount = masterVM.GradePages.Count;
            AudioItemCount = masterVM.GradePages.Select(gradePage => gradePage.GradeItems.Count).Sum();
            DataContext = masterVM.GradePages[pageIndex];
            //mediaElement.Source = new Uri(GradePageVM.RefAudioPath);

            Loaded += (sender, e) =>
            {
                masterVM.AutoSaveResults();
            };

            CompositionTarget.Rendering += (sender, e) =>
            {
                //if (!_progressSync && playButton.IsChecked == true)
                //{
                //    _progressSync = true;
                //    timelineSlider.Value = mediaElement.Position.TotalSeconds;
                //    _progressSync = false;
                //}
            };
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _loadedTime = DateTime.Now;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            //playButton.IsChecked = false;
            //mediaElement.Stop();
            //timelineSlider.Value = 0;

            var unloadedTime = DateTime.Now;
            var newPageElapsedTime = unloadedTime - _loadedTime;
            GradePageVM.PageElapsedTime += newPageElapsedTime;
        }

        private void OnPlayButtonChecked(object sender, RoutedEventArgs e) { }// => mediaElement.Play();
        private void OnPlayButtonUnchecked(object sender, RoutedEventArgs e) { }// => mediaElement.Pause();

        private void OnNextGradingPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM.GradeItems.All(gradeItem => gradeItem.Grade > 0))
                OnNextPageButtonClicked(sender, e);

            else
                MessageBox.Show(
                    App.Current.GetLocalizedString("S.GradePage.MsgBox.PleaseGradeAllBeforeNext"),
                    App.Current.GetLocalizedString("S.GradePage.MsgBox.GradeVerification"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnRefAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM == null) return;
            //mediaElement.Source = new Uri(GradePageVM.RefAudioPath);
        }

        private void OnAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            var gradeItemVM = (GradeItemVM)((RadioButton)sender).DataContext;
            //mediaElement.Source = new Uri(gradeItemVM.AudioPath);
        }

        private void OnMediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            //timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void OnTimelineSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (!_progressSync)
            //{
            //    _progressSync = true;
            //    mediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            //    _progressSync = false;
            //}
        }

        private void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            //if (loopButton.IsChecked == true)
            //    mediaElement.Position = TimeSpan.Zero;

            //else
            //{
            //    playButton.IsChecked = false;
            //    mediaElement.Stop();
            //    timelineSlider.Value = 0;
            //}
        }
    }
}

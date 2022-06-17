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

namespace MushraSharp
{
    public partial class GradingPage : PageWithNext
    {
        public GradePageVM GradePageVM => (GradePageVM)DataContext;
        public int PageCount { get; init; }

        DateTime _loadedTime;

        public GradingPage(MasterVM masterVM, int pageIndex) : base(masterVM, pageIndex)
        {
            InitializeComponent();
            PageCount = masterVM.GradePages.Count;
            DataContext = masterVM.GradePages[pageIndex];
            //mediaElement.Source = new Uri(GradePageVM.RefAudioPath);

            Loaded += (sender, e) =>
            {
                masterVM.AutoSaveResults();
            };
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _loadedTime = DateTime.Now;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            audioPlayer.IsPlaying = false;
            audioPlayer.PlaybackPosition = 0;

            var unloadedTime = DateTime.Now;
            var newPageElapsedTime = unloadedTime - _loadedTime;
            GradePageVM.PageElapsedTime += newPageElapsedTime;
        }

        private void OnNextGradingPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM.GradeItems.Any(gradeItem => gradeItem.Grade == 100))
                OnNextPageButtonClicked(sender, e);

            else
                MessageBox.Show(
                    App.Current.GetLocalizedString("S.GradePage.MsgBox.AtLeastOneGrade100Required"),
                    App.Current.GetLocalizedString("S.GradePage.MsgBox.GradeVerification"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnRefAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            if (GradePageVM == null) return;
            audioPlayer.AudioSource = GradePageVM.RefAudioSource;
        }

        private void OnAudioPlaybackChecked(object sender, RoutedEventArgs e)
        {
            var gradeItemVM = (GradeItemVM)((RadioButton)sender).DataContext;
            audioPlayer.AudioSource = gradeItemVM.AudioSource;
        }
    }
}

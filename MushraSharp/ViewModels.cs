using CommonKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MushraSharp
{
    public class GradeItemVM : DependencyObject
    {
        public static readonly DependencyProperty GradeProperty =
            DependencyProperty.Register(nameof(Grade), typeof(int), typeof(GradeItemVM),
                new FrameworkPropertyMetadata(50));

        Lazy<AudioSource> _audioSourceLazy;

        public int Grade
        {
            get => (int)GetValue(GradeProperty);
            set => SetValue(GradeProperty, value);
        }

        public string AudioPath { get; init; }
        public AudioSource AudioSource => _audioSourceLazy.Value;

        public GradeItemVM(string audioPath)
        {
            _audioSourceLazy = new Lazy<AudioSource>(() => AudioSource.FromFile(audioPath));
            AudioPath = audioPath;

            Task.Run(() =>
                GC.KeepAlive(AudioSource));
        }
    }

    public class GradePageVM : DependencyObject
    {
        Lazy<AudioSource> _refAudioSourceLazy;

        public string RefAudioPath { get; init; }
        public AudioSource RefAudioSource => _refAudioSourceLazy.Value;
        public List<GradeItemVM> GradeItems { get; init; }
        public List<GradeItemVM> ShuffledGradeItems { get; init; }

        public TimeSpan PageElapsedTime { get; set; }

        public GradePageVM(string refAudioPath, IEnumerable<GradeItemVM> gradeItems)
        {
            _refAudioSourceLazy = new Lazy<AudioSource>(() => AudioSource.FromFile(refAudioPath));
            RefAudioPath = refAudioPath;
            GradeItems = gradeItems.ToList();

            var rng = new Random();
            ShuffledGradeItems = GradeItems.OrderBy(_ => rng.Next()).ToList();

            Task.Run(() =>
                GC.KeepAlive(RefAudioSource));
        }
    }

    public class MasterVM : DependencyObject
    {
        public List<GradePageVM> GradePages { get; init; }

        public MasterVM(string audioFolder)
        {
            var rng = new Random();
            GradePages =
                Directory.EnumerateFiles(audioFolder, "*-ref.flac")
                .OrderBy(_ => rng.Next())
                .Select(refAudioPath => new GradePageVM(
                    refAudioPath,
                    Directory.EnumerateFiles(audioFolder, Path.GetFileName(refAudioPath)[..^"-ref.flac".Length] + "-*.flac")
                        .Select(audioPath => new GradeItemVM(audioPath))))
                .ToList();
        }

        public MasterVM() : this(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "audio")) { }

        public static MasterVM DesignerVM => new MasterVM(@"D:\Dm\MushraSharp\MushraSharp\bin\Debug\net5.0-windows\audio");
        public static GradePageVM DesignerGradePageVM => DesignerVM.GradePages[0];

        public string CompileResults()
        {
            var jStr = System.Text.Json.JsonSerializer.Serialize(new
            {
                Results = GradePages
                    .SelectMany(gradePage => gradePage.GradeItems)
                    .ToDictionary(
                        gradeItem => Path.GetFileNameWithoutExtension(gradeItem.AudioPath),
                        gradeItem => gradeItem.Grade),
                ElapsedTimes = GradePages
                    .Select(gradePage => new object[] {
                        Path.GetFileNameWithoutExtension(gradePage.RefAudioPath),
                        gradePage.PageElapsedTime.TotalSeconds })
                    .ToList(),
            });

            return jStr + Environment.NewLine;
        }

        public void AutoSaveResults()
        {
            var resultText = CompileResults();
            var resultTextSavePath = "_autosave_result.json";

            try
            {
                File.WriteAllText(resultTextSavePath, resultText, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{ex}",
                    App.Current.GetLocalizedString("S.FinishPage.MsgBox.AutoSaveFailed"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

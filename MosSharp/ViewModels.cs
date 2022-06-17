using CommonKit;
using CommonKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MosSharp
{
    public class GradeItemVM : DependencyObject
    {
        public static int[] MosGrades { get; } = new[] { 1, 2, 3, 4, 5 };

        public static IValueConverter MosValueToSelectedItemConverter { get; } = new LambdaConverter(
            (value, targetType, parameter, culture) => (value is int intValue && Array.IndexOf(MosGrades, intValue) >= 0) ? value : null,
            (value, targetType, parameter, culture) => value ?? 0);

        public static readonly DependencyProperty GradeProperty =
            DependencyProperty.Register(nameof(Grade), typeof(int), typeof(GradeItemVM),
                new FrameworkPropertyMetadata(0));

        Lazy<AudioSource> _audioSourceLazy;

        public int Grade
        {
            get => (int)GetValue(GradeProperty);
            set => SetValue(GradeProperty, value);
        }

        public int Index { get; init; }
        public string AudioPath { get; init; }
        public AudioSource AudioSource => _audioSourceLazy.Value;

        public int IndexOneBased => Index + 1;

        public GradeItemVM(int index, string audioPath)
        {
            _audioSourceLazy = new Lazy<AudioSource>(() => AudioSource.FromFile(audioPath));
            Index = index;
            AudioPath = audioPath;

            Task.Run(() =>
                GC.KeepAlive(AudioSource));
        }
    }

    public class GradePageVM : DependencyObject
    {
        public List<GradeItemVM> GradeItems { get; init; }
        public TimeSpan PageElapsedTime { get; set; }

        public GradePageVM(IEnumerable<GradeItemVM> gradeItems)
        {
            GradeItems = gradeItems.ToList();
        }
    }

    public class MasterVM : DependencyObject
    {
        public List<GradePageVM> GradePages { get; init; }

        public MasterVM(string audioFolder)
        {
            var rng = new Random();
            GradePages =
                Directory.EnumerateFiles(audioFolder, "*.flac")
                .OrderBy(_ => rng.Next())
                .Select((audioPath, index) => (audioPath, index))
                .Chunk(15)
                .Select(indexedAudioPaths => new GradePageVM(indexedAudioPaths.Select(indexedAudioPath =>
                    new GradeItemVM(indexedAudioPath.index, indexedAudioPath.audioPath))))
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
                    .Select(gradePage => gradePage.PageElapsedTime.TotalSeconds)
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

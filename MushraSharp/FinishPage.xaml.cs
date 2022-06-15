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

namespace MushraSharp
{
    public partial class FinishPage : Page
    {
        public MasterVM MasterVM => (MasterVM)DataContext;

        public FinishPage(MasterVM masterVM)
        {
            InitializeComponent();
            DataContext = masterVM;

            Loaded += (sender, e) =>
            {
                var resultText = masterVM.CompileResults();

                var resultTextSavePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "_autosave_result.json");
                try
                {
                    System.IO.File.WriteAllText(resultTextSavePath, resultText, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($@"{ex}",
                        App.Current.GetLocalizedString("S.FinishPage.MsgBox.AutoSaveFailed"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                resultTextBox.Text = resultText;
                resultTextBox.Focus();
            };

            resultTextBox.GotFocus += (sender, e) =>
            {
                resultTextBox.SelectAll();
            };
        }

        private void OnPrevPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void OnCopyResultButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(resultTextBox.Text);
                resultTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{resultTextBox.Text}\r\n\r\n{ex}",
                    App.Current.GetLocalizedString("S.FinishPage.MsgBox.CopyFailed"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

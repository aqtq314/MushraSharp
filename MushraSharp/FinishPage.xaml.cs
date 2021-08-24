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
                resultTextBox.Text = masterVM.CompileResults();
                resultTextBox.SelectAll();
                resultTextBox.Focus();
            };
        }

        private void OnPrevPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}

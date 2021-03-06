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
    public partial class MainWindow : Window
    {
        public MasterVM MasterVM => (MasterVM)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MasterVM();
            frame.Content = new StartPage(MasterVM);
        }

        private void OnLocaleListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ResourceDictionary resourceDictionary in e.AddedItems)
                App.Current.SetLocaleDictionary(resourceDictionary);
        }
    }
}

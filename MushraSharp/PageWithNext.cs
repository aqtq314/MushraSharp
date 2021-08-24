using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MushraSharp
{
    public class PageWithNext : Page
    {
        protected MasterVM MasterVM { get; init; }
        public int PageIndex { get; init; }

        public PageWithNext(MasterVM masterVM, int pageIndex = -1)
        {
            MasterVM = masterVM;
            PageIndex = pageIndex;
        }

        protected void OnPrevPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        protected void OnNextPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (PageIndex + 1 >= MasterVM.GradePages.Count)
                NavigationService.Navigate(new FinishPage(MasterVM));
            else
                NavigationService.Navigate(new GradingPage(MasterVM, PageIndex + 1));
        }
    }
}

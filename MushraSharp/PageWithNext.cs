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
        Page? _nextPage = null;

        protected MasterVM MasterVM { get; init; }
        public int PageIndex { get; init; }
        public int PageIndexOneBased => PageIndex + 1;

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
            if (_nextPage == null)
            {
                if (PageIndex + 1 >= MasterVM.GradePages.Count)
                    _nextPage = new FinishPage(MasterVM);
                else
                    _nextPage = new GradingPage(MasterVM, PageIndex + 1);
            }

            NavigationService.Navigate(_nextPage);
        }
    }
}

using System.Windows;
using Microsoft.Phone.Controls;
using OpenToDo.Services;

namespace OpenToDo.Views
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            DataContext = ViewModelLocatorService.GetViewModel(this.GetType());
            InitializeComponent();

            // To avoid flickering with the initial state, just before the storyboard is launched.
            //LayoutRoot.Opacity = 0;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // replace the code here with a behavior.
            //this.ForwardIn.Begin();
        }
    }
}
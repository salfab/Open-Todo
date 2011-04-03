using System;
using System.Threading;
using Microsoft.Phone.Controls;
using OpenToDo.ViewModels;

namespace OpenToDo.Views
{
    public partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Is used to bind to the EditTaskCommand as it was difficult to get to it through the DataContext property since FindAncestor doesn't exist in Silverlight.
            Resources.Add("ViewModel", DataContext);
            InitializeComponent();
        }


        /// <summary>
        /// Called when the ApplicationBar's Settings button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsButtonClicked(object sender, EventArgs e)
        {
            var dataContext = (MainViewModel) DataContext;
            if (dataContext != null)
            {                
                dataContext.ShowSettingsCommand.Execute();
            }
        }

        /// <summary>
        /// Called when the ApplicationBar's Add button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddButtonClicked(object sender, EventArgs e)
        {
            var dataContext = (MainViewModel)DataContext;
            if (dataContext != null)
            {
                dataContext.AddCommand.Execute();
            }
        }

        /// <summary>
        /// Called when the ApplicationBar's About button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AboutButtonClicked(object sender, EventArgs e)
        {
            var dataContext = (MainViewModel)DataContext;
            if (dataContext != null)
            {
                dataContext.ShowAboutCommand.Execute();
            }
        }

        /// <summary>
        /// Handles the BackKeyPress event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // FIXME : Even with this solution, it's not satisfying, as a user could click on the Windows button to leave the application, and it wouldn't call this event... We need to work with the App.Exit event, even if it makes some funny things when blocking a thread!
            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                App.ViewModel.PersistenceService.SendPushNotification(mre);

                // This is a dirty hack, but at least we were cautious not to put the system in deadlock : 
                // since Silverlight's BeginGetResponse is asynchronous AND uses a secondary thread ( the latter was a bad idea in the first place as it could have been executed on the main thread )
                // we would think it could be possible to raise a signal on a ManualResetEvent when the method returns, only Silverlight goes back and forth between the main and secondary thread even if
                // there's nothing to do there, effectively resulting in a deadlock if the MainThread is blocked.
                // launching the SendPushNotification from a secondary thread wouldn't change anything (anyway, as explained earlier, it's *already* running on a secondary thread ).
                // To show how inconsistent it is, BeginGetRequestStream which is also asynchronous and multithreaded (again : why the latter ? it could return on the main thread!) does not switch to the main thread.
                // That means that we can try to delay the ManualResetEvent.Set at leas *just before* we issue the push notification, and by the time the thread returns to the Main Thread, we can hope that the notification had the time to go through.
                // That's horrible, but that's just the best we can do with this 1.0 WP7 release :( salfab - 08.01.2011 
                mre.WaitOne(2000);
            }
        }
    }
}
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using OpenToDo.Services;
using OpenToDo.ViewModels;

namespace OpenToDo.Views
{
    public partial class EditTaskView
    {
        /// <summary>
        /// This field is used to temporary disable the Url argument setting the active task when navigating to this page.
        /// </summary>
        /// <remarks>
        /// It should be used when navigating to a full-screen dialog box such as a date picker, because upon navigation back to this page, the OnNavigated event will be raised, and we do *not* want to re-set the active task again, we want to keep the current one.</remarks>
        private bool _disableActiveTaskSetting;

        public EditTaskView()
        {
            EditTaskViewModel dataContext = (EditTaskViewModel) ViewModelLocatorService.GetViewModel(GetType());

            DataContext = dataContext;
            InitializeComponent();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.Uri.ToString() == "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml")
            {
                _disableActiveTaskSetting = true;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!_disableActiveTaskSetting)
            {
                var taskIndex = Int32.Parse(NavigationContext.QueryString["TaskIndex"]);
                ((EditTaskViewModel)DataContext).SetTaskIndexAsActive(taskIndex);
            }

            // it's disabled only as a one-shot.
            _disableActiveTaskSetting = false;
        }

        private void ValidateModifications(object sender, EventArgs eventArgs)
        {
            var elementToUpdate = (Control)FocusManager.GetFocusedElement();

            // If something has the focus, and therefore, needs to have a workaround applied for the unfriendly behavior of the ApplicationBar concerning the Bindings. ( remember ? the focus is not lost when clicking the ApplicationBar, resulting in the binding not being updated as it's done on loosing focus.
            if (elementToUpdate != null)
            {
                BindingUpdateHelper.RefreshBinding(elementToUpdate);                
            }

            var viewModel = (EditTaskViewModel) DataContext;
            viewModel.ValidateEditionCommand.Execute(null);            
            GoBack();
        }

        private void DeleteTask(object sender, EventArgs e)
        {
            var viewModel = (EditTaskViewModel) DataContext;
            viewModel.DeleteTaskCommand.Execute();
            GoBack();
        }

        /// <summary>
        /// Goes back to where we came from.
        /// </summary>
        /// <remarks>This method should be encapsulated in a View Management Service and be injected in the viewmodel's constructor for the sake of testability.</remarks>
        private void GoBack()
        {
            NavigationService.GoBack();
        }

        private void DatePickerValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            // Hack : The control is not yet very stable and the twoway bindings aren't quite working : This event handler is a little hacky workaround to make it work without a binding.
            var viewModel = (EditTaskViewModel)DataContext;
            if (e.NewDateTime.HasValue)
            {
                viewModel.Task.DueDate = e.NewDateTime.Value;
            }
  
        }

    }
}
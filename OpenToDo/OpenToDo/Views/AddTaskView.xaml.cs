using System;
using System.Windows.Controls;
using System.Windows.Input;
using OpenToDo.Services;
using OpenToDo.ViewModels;

namespace OpenToDo.Views
{
    public partial class AddTaskView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTaskView"/> class.
        /// </summary>
        public AddTaskView()
        {
            DataContext = ViewModelLocatorService.GetViewModel(GetType());
            InitializeComponent();
        }

        /// <summary>
        /// Proxy to the AddTaskCommand.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddTask(object sender, EventArgs e)
        {
            var elementToUpdate = (Control)FocusManager.GetFocusedElement();
            BindingUpdateHelper.RefreshBinding(elementToUpdate);
            
            var viewModel = (AddTaskViewModel) DataContext;
            viewModel.AddTaskCommand.Execute();
            
            // Fixme : Delegate this to a dedicated Service in the Command Handler.
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


    }
}
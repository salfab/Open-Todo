using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Shell;
using OpenToDo.Models;
using OpenToDo.Services;
using OpenToDo.ViewModels;

namespace OpenToDo.Views
{
    public partial class EditListsView
    {
        private IApplicationBarIconButton _applyButton;
        private IApplicationBarIconButton _deleteButton;
        private IApplicationBarIconButton _addButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditListsView"/> class.
        /// </summary>
        public EditListsView()
        {
            DataContext = ViewModelLocatorService.GetViewModel(GetType());


            this.Resources.Add("ViewModel", DataContext);

            InitializeComponent();

            // HACK: Again, big hack because the buttons cannot be accessed by their name. In an ideal This world, this should be done with triggers(WPF) or behaviors(Silverlight)
            _deleteButton = ApplicationBar.Buttons.Cast<IApplicationBarIconButton>().First(button => button.Text.Equals("delete", StringComparison.OrdinalIgnoreCase));
            _applyButton = ApplicationBar.Buttons.Cast<IApplicationBarIconButton>().First(button => button.Text.Equals("apply", StringComparison.OrdinalIgnoreCase));
            _addButton = ApplicationBar.Buttons.Cast<IApplicationBarIconButton>().First(button => button.Text.Equals("add", StringComparison.OrdinalIgnoreCase));
            
            // remove the edition buttons since we are in insertion mode by default
            InsertDefaultButtons();
        }

        /// <summary>
        /// Proxy to the AddTaskListCommand
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Because of the <see cref="ApplicationBar"/> + Binding issue, we are forced to twist the arm of the MVVM pattern in order to force the refresh of the binding for the last element to have the focus.</remarks>
        private void AddTaskList(object sender, EventArgs e)
        {
            var viewModel = (EditListsViewModel) DataContext;


            // If we were in edition now untill now : 
            if (viewModel.IsEditing)
            {
                // HACK: an other way of doing, if we don't want to use the commandParameter, but we have to refresh the bindings manually
                // ( and specify the UI Element type to refresh, which kind of defeats the point ) because the Application bar is not a managed UI, it's part
                // of the native UI Chrome, and thus, doesn't remove the focus from the textbox. And Silverlight doesn't provide a strategy of bindings which will get it updated on each keystroke...
                // so, now we're left with the responsibility to make sure the bindings are refreshed before using the bound value... Not so great, I'd say !
                var focused =  FocusManager.GetFocusedElement() as Control;
                if (focused != null)
                {
                    BindingUpdateHelper.RefreshBinding(focused);
                }
                viewModel.AddTaskListCommand.Execute();

                // the delete button is always at the last position.
                
                // Add the buttons for the consultation view.
                InsertDefaultButtons();                
            }
            else
            {
                // Make sure we have a clean slate : 
                viewModel.CurrentTaskList = new TaskList();

                // Remove buttons for the consultation view
                ApplicationBar.Buttons.Remove(_deleteButton);
            }

            // Let's set the current view mode to edition or consultation.
            viewModel.IsEditing = !viewModel.IsEditing;

        }

        /// <summary>
        /// Proxy to the DeleteTaskListCommand
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Deletes the tasklists selected in the taskLists control. : As <see cref="ApplicationBar"/> does not have support for Commands, we are forced to proxy the command execution in the view's code-behind.</remarks>
        private void RemoveList(object sender, EventArgs e)
        {
            var viewModel = (EditListsViewModel)DataContext;
            var commandParameter = viewModel.TaskLists.Where(o => o.IsChecked);
            viewModel.DeleteTaskListCommand.Execute(commandParameter);
        }

        private void ApplyTaskListChanges(object sender, EventArgs e)
        {
            var viewModel = (EditListsViewModel)DataContext;


            // If we were in edition now untill now : 
            if (viewModel.IsEditing)
            {
                // HACK: an other way of doing, if we don't want to use the commandParameter, but we have to refresh the bindings manually
                // ( and specify the UI Element type to refresh, which kind of defeats the point ) because the Application bar is not a managed UI, it's part
                // of the native UI Chrome, and thus, doesn't remove the focus from the textbox. And Silverlight doesn't provide a strategy of bindings which will get it updated on each keystroke...
                // so, now we're left with the responsibility to make sure the bindings are refreshed before using the bound value... Not so great, I'd say !
                var focused = FocusManager.GetFocusedElement() as Control;
                if (focused != null)
                {
                    BindingUpdateHelper.RefreshBinding(focused);
                }
                var commandParameter = viewModel.CurrentTaskList;
                viewModel.ApplyTaskListChangesCommand.Execute();           

                // Add the buttons for the consultation view.
                InsertDefaultButtons();

            }
            else
            {
                // the delete button is always at the last position.

                // Remove buttons for the consultation view
                ApplicationBar.Buttons.Remove(_deleteButton);

                // Add the buttons for the edition view.
                ApplicationBar.Buttons.Insert(0, _applyButton);
            }

            // Let's set the current view mode to edition or consultation.
           //viewModel.IsEditing = !viewModel.IsEditing;
        }

        private void UpdateApplicationBarButtonsForEdition(object sender, RoutedEventArgs e)
        {
            var viewModel = (EditListsViewModel)DataContext;

            if (viewModel.IsEditing)
            {
                //viewModel.EditTaskListCommand.Execute(viewModel.CurrentTaskList);


                //ApplicationBar.Buttons.Remove(_applyButton);

                // Add the buttons for the consultation view.
                ApplicationBar.Buttons.Add(_addButton);
                ApplicationBar.Buttons.Add(_deleteButton);

                // Remove buttons for the edition view
                ApplicationBar.Buttons.Remove(_applyButton);
            }
            else
            {
                // the delete button is always at the last position.

                // Remove buttons for the consultation view
                ApplicationBar.Buttons.Remove(_deleteButton);
                ApplicationBar.Buttons.Remove(_addButton);

                // Add the buttons for the edition view.
                ApplicationBar.Buttons.Insert(0, _applyButton);
            }
        }

        /// <summary>
        /// Hack to close the edit mode overlay.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>this should be removed</remarks>
        private void HackToCloseEditMode(object sender, CancelEventArgs e)
        {
            // TODO : Remove this event handler, use proper navigation instead.

            var viewModel = (EditListsViewModel)DataContext;
            if (viewModel.IsEditing)
            {
                e.Cancel = true;

                viewModel.IsEditing = false;

                ApplicationBar.Buttons.Clear();
                InsertDefaultButtons();
            }
        }

        private void InsertDefaultButtons()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(_addButton);
            ApplicationBar.Buttons.Add(_deleteButton);
        }
    }
}
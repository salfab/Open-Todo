using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using OpenToDo.Models;

namespace OpenToDo.ViewModels
{
    public class EditListsViewModel : INotifyPropertyChanged
    {
        private bool _isEditing;
        private string _taskListName;
        private TaskList _currentTaskList;

        public EditListsViewModel(ObservableCollection<TaskList> taskLists)
        {
            TaskLists = taskLists;
            CurrentTaskList = new TaskList();

            AddTaskListCommand = new DelegateCommand(AddTaskListCommandHandler);
            EditTaskListCommand = new DelegateCommand<TaskList>(EditTaskListCommandHandler);
            DeleteTaskListCommand = new DelegateCommand<IEnumerable<TaskList>>(DeleteTaskListCommandHandler);
            ApplyTaskListChangesCommand = new DelegateCommand(ApplyTaskListChangesCommandHandler);
        }

        private void ApplyTaskListChangesCommandHandler()
        {
            // For the tasklist being edited : replace the original by the modified clone
            var taskListToRemove = TaskLists.Single(o => o.ListId == CurrentTaskList.ListId);
            TaskLists.Remove(taskListToRemove);
            TaskLists.Add(CurrentTaskList);

            IsEditing = false;
            CurrentTaskList = new TaskList();
        }

        private void EditTaskListCommandHandler(TaskList taskList)
        {
            // For the task list being edited : create a clone we can modify. (for rollback purposes)

            CurrentTaskList = taskList.Clone();
            // Let's set the current view mode to edition or consultation.
            IsEditing = !IsEditing;
        }

        public ObservableCollection<TaskList> Type { get; private set; }

        public DelegateCommand AddTaskListCommand { get; set; }

        public DelegateCommand<IEnumerable<TaskList>> DeleteTaskListCommand { get; set; }

        public ObservableCollection<TaskList> TaskLists { get; private set; }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                OnPropertyChanged("IsEditing");
            }
        }

        public string TaskListName
        {
            get { return _taskListName; }
            set
            {
                _taskListName = value;
                OnPropertyChanged("TaskLists");
            }
        }

        public DelegateCommand<TaskList> EditTaskListCommand { get; set; }

        public TaskList CurrentTaskList
        {
            get {
                return _currentTaskList;
            }
            set
            {
                _currentTaskList = value;
                OnPropertyChanged("CurrentTaskList");
            }
        }

        public DelegateCommand ApplyTaskListChangesCommand { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void DeleteTaskListCommandHandler(IEnumerable<TaskList> list)
        {
            // Using a foreach loop would cause some side effects, so let's just use a decrementing for loop.
            for (int i = list.Count()-1; i >= 0; i--)
            {
                TaskLists.Remove(list.ElementAt(i));
            }
        }

        private void AddTaskListCommandHandler()
        {
            TaskLists.Add(CurrentTaskList);
            CurrentTaskList = new TaskList();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using OpenToDo.Models;

namespace OpenToDo.ViewModels
{
    public class EditTaskViewModel : INotifyPropertyChanged
    {
        private Task _task;
        public Task Task
        {
            get { return _task; }
            private set 
            { 
                _task = value; 
                OnPropertyChanged("Task");
            }
        }

        private readonly ObservableCollection<Task> _tasks;
        private ObservableCollection<TaskList> _taskLists;

        public ObservableCollection<TaskList> TaskLists
        {
            get { return _taskLists; }
            private set
            {
                _taskLists = value;
                OnPropertyChanged("TaskLists");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand ValidateEditionCommand { get; set; }

        public TaskList SelectedTaskList { get; set; }

        private string _taskName;

        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                OnPropertyChanged("TaskName");
            }
        }

        public DelegateCommand DeleteTaskCommand { get; set; }

        public EditTaskViewModel(ObservableCollection<Task> tasks, ObservableCollection<TaskList> tasksList)
        {            
            _tasks = tasks;
            _taskLists = tasksList;
            ValidateEditionCommand = new DelegateCommand(OnValidateEdition);
            DeleteTaskCommand = new DelegateCommand(OnDeleteTask);
        }

        private void OnDeleteTask()
        {
            // Since we're using a clone of the task to edit ( in order to roll back and cancel the modifications we might have done )
            // we have to look for the task which has the same InternalId.
            _tasks.Remove(_tasks.Single(o => o.InternalId == Task.InternalId));
        }

        /// <summary>
        /// Sets the task index as active.
        /// </summary>
        /// <param name="taskIndex">Index of the task.</param>
        /// <remarks>
        /// Note : This method call is not thread safe in the following scenario :
        /// A call to the method is done, and when entering the method, the control is lost to an other thread, which would update the list of tasks,
        /// resulting in an offset of the tasks indices, thus selecting the wrong task to edit.
        /// Fortunately, selecting the task from the list, resulting to this method call are always done on the UI thread, so it should minimize the risks.
        /// </remarks>
        public void SetTaskIndexAsActive(int taskIndex)
        {
            // Optimization : we keep only one VM and we just pass the ID of the active task as a URL parameter during the navigation. The view then sets the active task within the viewmodel. It helps keeping a smaller memory footprint.
            Task = _tasks[taskIndex].Clone();

            TaskName = Task.TaskName;
            SelectedTaskList = _taskLists.SingleOrDefault(x => Task.TaskListId == x.ListId);
        }

        private void OnValidateEdition()
        {            
            // TODO: Add a behavior so we can workaround the lack of bindability of the SelectedItems property (which is not a Dependency Property) and be able to deal with multiple selections
            if (SelectedTaskList != null)
            {
                Task.TaskListId = SelectedTaskList.ListId;
            }

            // InternalID is exposed so we don't have to rely on taskIndexes which would haven't been thread safe.
            var taskToRemove = _tasks.Single(o => o.InternalId == Task.InternalId);

            // avoid losing control while messing with the indices. it's not about the RemoveAt, but rather about the Insert : just to make sure it's inserted at the right position.
            lock (taskToRemove)
            {
                var index = _tasks.IndexOf(taskToRemove);
                _tasks.RemoveAt(index);
                _tasks.Insert(index, Task);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Practices.Prism.Commands;
using OpenToDo.Models;

namespace OpenToDo.ViewModels
{
    public class AddTaskViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Task> _tasks;
        public ObservableCollection<TaskList> TaskLists { get; private set; }
        public string ViewTitle { get; set; }

        private Task _task;
        public Task Task
        {
            get
            {
                return _task;
            }
            private set
            {
                _task = value;
                OnPropertyChanged("Task");
            }
        }

        public AddTaskViewModel(ObservableCollection<Task> tasks, ObservableCollection<TaskList> taskLists)
        {
            _tasks = tasks;
            TaskLists = taskLists;
            ViewTitle = "add new task";

            Task = new Task();

            AddTaskCommand = new DelegateCommand(AddTaskCommandHandler);
        }

        private void AddTaskCommandHandler()
        {
            Task taskToAdd;

            // Make sure nobody edits the Task before it's been reset.
            lock (_tasks)
            {
                taskToAdd = Task;
                Task = new Task();    
            }

            _tasks.Add(taskToAdd);
            
        }

        public DelegateCommand AddTaskCommand { get; set; }

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

        public TaskList SelectedTaskList { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

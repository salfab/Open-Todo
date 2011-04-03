using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using OpenToDo.ViewModels;

namespace OpenToDo.Models
{
    [DataContract]
    public sealed class Task : INotifyPropertyChanged
    {
        private string _description;
        private DateTime _dueDate;
        private bool _isDone;
        private TaskPriority _priority;
        private string _shortInfo;
        private Guid _taskListId;
        private string _taskName;

        public Task(string taskName, Guid listId)
        {
            InternalId = Guid.NewGuid();
            DueDate = DateTime.Today;

            if (taskName == null)
            {
                throw new ArgumentNullException("taskName");
            }
            TaskListId = listId;
            TaskName = taskName;
        }

        public Task() : this(string.Empty, Guid.Empty)
        {            
        }


        [DataMember]
        public Guid InternalId { get; set; }

        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        [DataMember]
        public string ShortInfo
        {
            get
            {
                return _shortInfo;
            }
            set
            {
                _shortInfo = value;
                OnPropertyChanged("ShortInfo");
            }
        }

        [DataMember]
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                OnPropertyChanged("TaskName");
            }
        }

        [DataMember]
        public Guid TaskListId
        {
            get { return _taskListId; }
            set
            {
                _taskListId = value;
                OnPropertyChanged("TaskListId");
            }
        }

        [DataMember]
        public TaskPriority Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                OnPropertyChanged("Priority");
            }
        }

        [DataMember]
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        [DataMember]
        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                _isDone = value;
                OnPropertyChanged("IsDone");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Task Clone()
        {
            return (Task) MemberwiseClone();
        }
    }
}
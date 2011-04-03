using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace OpenToDo.Models
{
    //[System.Runtime.Serialization.DataContractAttribute]
    [DataContract]
    public class TaskList : INotifyPropertyChanged
    {
        public TaskList(string taskListName)
        {
            Name = taskListName;
            ListId = Guid.NewGuid();
        }

        public TaskList() : this(null)
        {            
        }

        [DataMember]
        public Guid ListId { get; set; }

        private string _name;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public TaskList Clone()
        {
            return (TaskList)MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
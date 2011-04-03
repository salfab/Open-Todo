using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OpenToDo.Models;

namespace OpenToDo.ViewModels
{
    public class FiltersViewModel : INotifyPropertyChanged
    {
        private readonly Func<ObservableCollection<Task>, IEnumerable<Task>> _filterPredicate;
        private readonly ObservableCollection<Task> _tasks;

        /// <summary>
        /// The backing field for the <see cref="FilterName"/> property.
        /// </summary>
        private string _filterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersViewModel"/> class.
        /// </summary>
        /// <param name="filterPredicate">The filter predicate to define how the tasks are going to be filtered to determine whether they will be part of it.</param>
        /// <param name="tasks">The tasks to filter and optionnaly, add to the filter.</param>
        public FiltersViewModel(Func<ObservableCollection<Task>, IEnumerable<Task>> filterPredicate, ObservableCollection<Task> tasks)
        {
            _filterPredicate = filterPredicate;
            _tasks = tasks;
            // When a new task is added, we need to get the predicate to re-execute in order to include the new task.
            _tasks.CollectionChanged += (o, e) => OnPropertyChanged("Tasks");
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string FilterName
        {
            get
            {
                return _filterName;
            }
            set
            {
                if (value != _filterName)
                {
                    _filterName = value;
                    OnPropertyChanged("FilterName");
                }
            }
        }


        public IEnumerable<Task> Tasks
        {
            get
            {
                return _filterPredicate(_tasks);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
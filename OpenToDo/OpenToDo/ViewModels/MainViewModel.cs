using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Microsoft.Practices.Prism.Commands;
using OpenToDo.Models;
using OpenToDo.Services;
using OpenToDo.Views;

namespace OpenToDo.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The backing field for the <see cref="PersistenceService"/> property.
        /// </summary>
        private readonly IPersistenceService _persistenceService;

        /// <summary>
        /// A list of the task lists handled by the persistence service.
        /// </summary>
        private readonly ObservableCollection<TaskList> _taskLists;
        
        /// <summary>
        /// A list of the task lists handled by the persistence service.
        /// </summary>
        private readonly ObservableCollection<Task> _tasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            ApplicationName = "OPEN TODO";
            Filters = new ObservableCollection<FiltersViewModel>();

            EditTaskCommand = new DelegateCommand<Task>(OnEditCommandExecuted);
            AddCommand = new DelegateCommand(OnAddCommandExecuted);

            _persistenceService = new XmlPersistenceService();
            _taskLists = _persistenceService.GetTaskLists();
            _tasks = _persistenceService.GetTasks();

            // refresh the list of filters. i.e. fixed labels + taskLists
            _taskLists.CollectionChanged += (o, e) => InitFilters(_tasks);

            ViewModelLocatorService.SetViewModel(typeof(EditListsView), new EditListsViewModel(_taskLists));
            ViewModelLocatorService.SetViewModel(typeof(AddTaskView), new AddTaskViewModel(_tasks, _taskLists));    
            ViewModelLocatorService.SetViewModel(typeof(EditTaskView), new EditTaskViewModel(_tasks, _taskLists));    
            ViewModelLocatorService.SetViewModel(typeof(About), new AboutViewModel());    
        
            ShowAboutCommand = new DelegateCommand(OnShowAboutCommand);
            ShowSettingsCommand  = new DelegateCommand(OnShowSettings);
        }



        /// <summary>
        /// Gets or sets the edit task command.
        /// </summary>
        /// <value>The edit task command.</value>
        public DelegateCommand<Task> EditTaskCommand { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the show about command.
        /// </summary>
        /// <value>The show about command.</value>
        public DelegateCommand ShowAboutCommand { get; set; }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<FiltersViewModel> Filters { get; private set; }

        /// <summary>
        /// Gets the persistence service.
        /// </summary>
        /// <value>The persistence service.</value>
        public IPersistenceService PersistenceService
        {
            get { return _persistenceService; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data of this view model are loaded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the view model has its data loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the show settings command.
        /// </summary>
        /// <value>The show settings command.</value>
        public DelegateCommand ShowSettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets the add command.
        /// </summary>
        /// <value>The command to execute when adding a new task.</value>        
        public DelegateCommand AddCommand { get; set; }

        /// <summary>
        /// Gets or sets the edit command.
        /// </summary>
        /// <value>The command to execute when editing an existing task.</value>
        public DelegateCommand<Task> EditCommand { get; set; }

        /// <summary>
        /// Gets or sets the channel URI for the live tile.
        /// </summary>
        /// <value>The channel URI to microsoft's push notification server.</value>
        public Uri ChannelUri { get; set; }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Called when the edit command is executed.
        /// </summary>
        /// <param name="task">The task.</param>
        private void OnEditCommandExecuted(Task task)
        {
            if (EditTaskCommand.CanExecute(task))
            {
                string uriString = "/Views/EditTaskView.xaml?TaskIndex="+_tasks.IndexOf(task);
                NavigateTo(uriString);
            }
        }

        /// <summary>
        /// Called when the Add task command is executed.
        /// </summary>
        private void OnAddCommandExecuted()
        {
            string uriString = "/Views/AddTaskView.xaml";

            NavigateTo(uriString);
        }

        /// <summary>
        /// Called when the show settings command is called.
        /// </summary>
        private void OnShowSettings()
        {
            if (ShowSettingsCommand.CanExecute())
            {
                string uriString = "/Views/EditListsView.xaml";
                NavigateTo(uriString);
            }
        }

        /// <summary>
        /// Called when the show about page command is triggered.
        /// </summary>
        private void OnShowAboutCommand()
        {
            string uriString = "/Views/About.xaml";

            NavigateTo(uriString);
        }

        /// <summary>
        /// Starts the navigation process with transition animations.
        /// </summary>
        /// <param name="destinationUri">The destination URI string.</param>
        private void NavigateTo(string destinationUri)
        {
            PhoneApplicationFrame rootFrame = ((App)Application.Current).RootFrame;
            //var forwardOut = ((MainPage)(rootFrame.Content)).ForwardOut;

            // fixme : this might not be threadsafe.
            //EventHandler forwardOutOnCompleted = null;
            //forwardOutOnCompleted = (o, e) =>
            //                            {
            //                                forwardOut.Completed -= forwardOutOnCompleted;
            rootFrame.Navigate(new Uri(destinationUri, UriKind.Relative));
            //                            };

            //forwardOut.Completed += forwardOutOnCompleted;
            //forwardOut.Begin();

            //// restore the screen to where it was at start in case we browse back.
            //rootFrame.Navigated += (o, e) => forwardOut.Stop();
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // deserialize data.
            ObservableCollection<Task> tasks = _persistenceService.GetTasks();

            foreach (Task task in tasks)
            {
                task.PropertyChanged += TaskChanged;
            }
            tasks.CollectionChanged += (o, e) =>
                                           {
                                               if (e.NewItems != null)
                                               {
                                                   foreach (Task newItem in e.NewItems)
                                                   {
                                                       newItem.PropertyChanged += TaskChanged;
                                                   }
                                               }

                                               if (e.OldItems != null)
                                               {
                                                   foreach (Task oldItem in e.OldItems)
                                                   {
                                                       oldItem.PropertyChanged -= TaskChanged;
                                                   }
                                               }
                                           };

            InitFilters(tasks);

            this.IsDataLoaded = true;
            
        }

        /// <summary>
        /// Inits the filters.
        /// </summary>
        /// <remarks>the filters are initialized to contain : the built-in filters (todo, done, all) plus the custom task lists.</remarks>
        /// <param name="tasks">The tasks.</param>
        private void InitFilters(ObservableCollection<Task> tasks)
        {
            Filters.Clear();
            
            var filterAllViewModel = new FiltersViewModel(o => o, tasks) { FilterName = "all" };
            Filters.Add(filterAllViewModel);
            
            var filterTodoViewModel = new FiltersViewModel(o=> o.Where(x => x.IsDone == false), tasks) { FilterName = "to do"};
            Filters.Add(filterTodoViewModel);            

            var filterDoneViewModel = new FiltersViewModel(o=> o.Where(x => x.IsDone), tasks) { FilterName = "done" };            
            Filters.Add(filterDoneViewModel);

            foreach (var taskList in _taskLists)
            {
                Guid listId = taskList.ListId;
                var taskListFilterViewModel = new FiltersViewModel(o=>o.Where(x=> x.TaskListId == listId),tasks) { FilterName = taskList.Name };

                Filters.Add(taskListFilterViewModel);
            }

            OnPropertyChanged("Filters");
        }

        /// <summary>
        /// Occurs when a task has one of its properties changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TaskChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (FiltersViewModel filtersViewModel in Filters)
            {
                filtersViewModel.OnPropertyChanged("Tasks");
            }

            // When a task is edited, the changes must be saved.
            _persistenceService.SaveState();
        }

        /// <summary>
        /// The method to call then a property is changed and a PropertyChanged event must be raised.
        /// </summary>
        /// <param name="filtersPropertyName">Name of the filters property.</param>
        protected void OnPropertyChanged(string filtersPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(filtersPropertyName));
            }
        }
    }
}
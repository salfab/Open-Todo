using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using OpenToDo.Models;

namespace OpenToDo.Services
{
    public class XmlPersistenceService : IPersistenceService
    {
        private readonly IsolatedStorageSettings _appState = IsolatedStorageSettings.ApplicationSettings;
        
        private ObservableCollection<Task> _tasks;

        public void SaveState()
        {
            _appState.Save();
        }

        public Guid GetDeviceId()
        {
            Guid deviceId;
            if (_appState.Contains("DeviceId"))
            {
                deviceId = (Guid)_appState["DeviceId"];
            }
            else
            {
                deviceId = Guid.NewGuid();
                _appState.Add("DeviceId", deviceId);
                _appState.Save();
            }

            return deviceId;
        }

        /// <summary>
        /// Gets the previously saved tasks.
        /// </summary>
        /// <remarks>The method is not thread-safe.</remarks>
        /// <returns>An observable collection of tasks which should be displayed as check-lists in the UI.</returns>
        public ObservableCollection<Task> GetTasks()
        {      
            if (_tasks != null)
            {
                return _tasks;
            }

            const string tasksKey = "Tasks";
            if (_appState.Contains(tasksKey))
            {
                _tasks = (ObservableCollection<Task>)_appState[tasksKey];
            }
            else
            {
                _tasks = new ObservableCollection<Task>();
                _appState.Add(tasksKey, _tasks);
            }

            // Notifications commented-out because we used a dirty hack to notify when exiting the application instead.
            //foreach (Task task in _tasks)
            //{
            //    task.PropertyChanged += TaskChanged;
            //}

            _tasks.CollectionChanged += (o, e) =>
                                            {
                                                SaveState();
                                                // Notifications commented-out because we used a dirty hack to notify when exiting the application instead.
                                                //if (e.NewItems != null)
                                                //{
                                                //    foreach (Task newItem in e.NewItems)
                                                //    {
                                                //        newItem.PropertyChanged += TaskChanged;
                                                //    }
                                                //}

                                                //if (e.OldItems != null)
                                                //{
                                                //    foreach (Task oldItem in e.OldItems)
                                                //    {
                                                //        oldItem.PropertyChanged -= TaskChanged;
                                                //    }
                                                //}
                                                
                                                //// send a notification if a new task is added.
                                                //SendPushNotification();
                                            };

            return _tasks;
        }

        // Notifications commented-out because we used a dirty hack to notify when exiting the application instead.
        //private void SendPushNotification()
        //{
        //    SendPushNotification(null);
        //}

        // Notifications commented-out because we used a dirty hack to notify when exiting the application instead.
        //private void TaskChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    // Send a notification if the IsDone flag is changed
        //    if (e.PropertyName == "IsDone")
        //    {
        //        SendPushNotification();
        //    }
        //}

        public void SendPushNotification(ManualResetEvent synchronizationToken)
        {
            if (App.ViewModel.ChannelUri == null)
            {
                // Channel not ready yet.
                if (synchronizationToken != null)
                {
                    synchronizationToken.Set();
                }
                return;
            }


            Task nextDueTask = _tasks.Where(o => o.IsDone == false).OrderBy(x => x.DueDate).FirstOrDefault();
            if (nextDueTask == null)
            {
                if (synchronizationToken != null)
                {
                    synchronizationToken.Set();
                }
                return;
            }
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                 "<wp:Notification xmlns:wp=\"WPNotification\">" +
                                 "<wp:Tile>" +
                //"<wp:BackgroundImage><background image path></wp:BackgroundImage>" +
                                 "<wp:Count>" + _tasks.Count(o => o.IsDone == false) + "</wp:Count>" +
                  "<wp:Title>Next : " + nextDueTask.DueDate.ToShortDateString() + "</wp:Title>" +
               "</wp:Tile> " +
            "</wp:Notification>";

          
            // The URI that the Push Notification Service returns to the Push Client when creating a notification channel.
            var sendNotificationRequest = (HttpWebRequest)WebRequest.Create(App.ViewModel.ChannelUri);

            // HTTP POST is the only allowed method to send the notification.
            sendNotificationRequest.Method = "POST";

            // The optional custom header X-MessageID uniquely identifies a notification message. If it is present, the // same value is returned in the notification response. It must be a string that contains a UUID.
            sendNotificationRequest.Headers["X-MessageID"] = Guid.NewGuid().ToString();
            sendNotificationRequest.ContentType = "text/xml";
            sendNotificationRequest.Headers["X-WindowsPhone-Target"] = "token";
            sendNotificationRequest.Headers["X-NotificationClass"] = "1";


            // Sets the notification payload to send.
            byte[] notificationMessage = Encoding.UTF8.GetBytes(payload);

            // Sets the web request content length.
            sendNotificationRequest.BeginGetRequestStream(iar =>
            {
                using (Stream requestStream = sendNotificationRequest.EndGetRequestStream(iar))
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);

                if (synchronizationToken != null)
                {
                    synchronizationToken.Set();
                }

                // Sends the notification and gets the response.
                sendNotificationRequest.BeginGetResponse(iar2 =>
                {
                    //using (HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.EndGetResponse(iar2))
                    //{

                    //    string notificationStatus = response.Headers["X-NotificationStatus"];
                    //    string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                    //    string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
                    //}
                }, null);

            }, null);           
        }


        /// <summary>
        /// Gets the previously saved task lists.
        /// </summary>
        /// <returns>An observable collection of task-lists. It represents the available categories which can be used to tag a task.</returns>
        /// <remarks>The method is not thread-safe.</remarks>
        public ObservableCollection<TaskList> GetTaskLists()
        {
            ObservableCollection<TaskList> taskLists;
            const string taskListsKey = "TaskLists";
            if (_appState.Contains(taskListsKey))
            {
                taskLists = (ObservableCollection<TaskList>)_appState[taskListsKey];
            }
            else
            {
                taskLists = new ObservableCollection<TaskList>();
                _appState.Add(taskListsKey, taskLists);
            }
            taskLists.CollectionChanged += (o, e) => SaveState();

            return taskLists;
        }
    }
}

using System.Collections.ObjectModel;
using System.Threading;
using OpenToDo.Models;

namespace OpenToDo.Services
{
    public interface IPersistenceService
    {
        void SaveState();
        ObservableCollection<Task> GetTasks();
        ObservableCollection<TaskList> GetTaskLists();
        void SendPushNotification(ManualResetEvent mre);
    }
}
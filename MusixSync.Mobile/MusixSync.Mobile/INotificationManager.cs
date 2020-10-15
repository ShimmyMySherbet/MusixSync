using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Mobile
{
    public interface INotificationManager
    {
        void Initialize();

        int ScheduleNotification(string title, string message);

    }
}

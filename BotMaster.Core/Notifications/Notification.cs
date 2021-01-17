using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Core.Notifications
{
    public abstract class Notification
    {
        public NotificationTarget Target { get; set; }
        public string TargetId { get; set; }
    }
}

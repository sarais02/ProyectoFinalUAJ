using System;

namespace TrackerG5
{
    [Serializable]

    internal class LogoutEvent: TrackerEvent
    {
        public LogoutEvent()
            : base()
        {
            typeEvent = "LogoutEvent";
        }
    }
}

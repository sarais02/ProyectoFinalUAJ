using System;

namespace TrackerG5
{
    [Serializable]
    internal class LoginEvent: TrackerEvent
    {
        public LoginEvent()
            : base()
        {
            typeEvent = "LoginEvent";
        }
    }
}

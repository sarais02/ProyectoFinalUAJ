using System;
using System.Collections.Generic;

namespace TrackerG5
{
    [Serializable]

    internal class EndTestEvent : TrackerEvent
    {
        public EndTestEvent()
            : base()
        {
            typeEvent = "EndTestEvent";
        }
    }
}


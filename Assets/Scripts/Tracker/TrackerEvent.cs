using System;
using System.Collections.Generic;

namespace TrackerG5
{
    [Serializable]
    internal class TrackerEvent
    {
        protected string typeEvent;

        string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        string idSession;
        public string IdSession
        {
            get { return idSession; }
            set { idSession = value; }
        }
        long timestamp;
        public long Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public virtual bool SetParamns(Dictionary<string, string> paramns)
        {
            return true;
        }


        public TrackerEvent()
        {

        }

        public string ToJson()
        {
            return "";
        }
    }
}

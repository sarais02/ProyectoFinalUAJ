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

        string idUser;
        public string IdUser
        {
            get { return idUser; }
            set { idUser = value; }
        }

        string idSession;
        public string IdSession
        {
            get { return idSession; }
            set { idSession = value; }
        }

        uint idLevel;
        public uint IdLevel
        {
            get { return idLevel; }
            set { idLevel = value; }
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

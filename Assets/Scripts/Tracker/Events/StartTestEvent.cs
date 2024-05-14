using System;
using System.Collections.Generic;


namespace TrackerG5
{
    [Serializable]

    internal class StartTestEvent : TrackerEvent
    {

        uint nBots;
        public uint NBots
        {
            get { return nBots; }
            set { nBots = value; }
        }


        public StartTestEvent()
            : base()
        {
            typeEvent = "StartTestEvent";
        }
        public override bool SetParamns(Dictionary<string, string> paramns)
        {
            base.SetParamns(paramns);

            if (!paramns.ContainsKey("nBots")
                || !uint.TryParse(paramns["nBots"], out nBots))
                return false;

            return true;
        }
    }
}


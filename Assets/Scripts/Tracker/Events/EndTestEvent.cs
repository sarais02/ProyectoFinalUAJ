using System;
using System.Collections.Generic;

namespace TrackerG5
{
    [Serializable]

    internal class EndTestEvent : TrackerEvent
    {

        float mapAchieve;

        public EndTestEvent()
            : base()
        {
            typeEvent = "EndTestEvent";
        }

        public override bool SetParamns(Dictionary<string, string> paramns)
        {
            base.SetParamns(paramns);

            if (!paramns.ContainsKey("mapAchieve")
                || !float.TryParse(paramns["mapAchieve"], out mapAchieve))
                return false;

            return true;
        }
    }
}


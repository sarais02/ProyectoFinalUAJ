using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace TrackerG5
{
    [Serializable]

    internal class BotPositionEvent : TrackerEvent
    {

        float posX;
        float posZ;
        string updateByMove;

        public BotPositionEvent()
            : base()
        {
            typeEvent = "BotPositionEvent";
        }

        public override bool SetParamns(Dictionary<string, string> paramns)
        {
            base.SetParamns(paramns);
           
            if(!paramns.ContainsKey("posX") || !float.TryParse(paramns["posX"], out posX) 
                || !paramns.ContainsKey("updateByMove") || !paramns.ContainsKey("posZ")
                || !float.TryParse(paramns["posZ"], out posZ))
                return false;

            updateByMove = paramns["updateByMove"];

            return true;
        }
    }
}


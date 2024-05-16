using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrackerG5
{
    [Serializable]

    internal class CPUUseEvent : TrackerEvent
    {
        
        float fps;
        float cpuUse;
        long memoryUse;


        public CPUUseEvent()
            : base()
        {
            typeEvent = "CPUUseEvent";
        }

        public override bool SetParamns(Dictionary<string, string> paramns)
        {
            base.SetParamns(paramns);

            if (!paramns.ContainsKey("fps") || !float.TryParse(paramns["fps"], out fps)
                || !paramns.ContainsKey("cpuUse") || !float.TryParse(paramns["cpuUse"], out cpuUse)
                || !paramns.ContainsKey("memoryUse") || !long.TryParse(paramns["memoryUse"], out memoryUse))
                return false;

            return true;
        }
    }
}


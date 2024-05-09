using System;


namespace TrackerG5
{
    internal interface ITrackerAsset
    {
        public bool accept(TrackerEvent e);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerG5
{
    internal interface IPersistence
    {
        void Send(TrackerEvent e);

        void Flush();

        void EndSession();
    }
}

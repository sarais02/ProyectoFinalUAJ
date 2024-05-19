using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TrackerG5
{
    internal class FilePersistence : IPersistence
    {
        List<TrackerEvent> eventsQueue = new List<TrackerEvent>();
        ISerializer mySerializer;
        FileStream fs;
        uint maxSizeQueue;
        public FilePersistence(ISerializer serializer, string route, uint maxSizeQueue)
        {
            this.maxSizeQueue = maxSizeQueue;
            mySerializer = serializer;

            fs = new FileStream(route, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            mySerializer.OpenFile(fs);
        }

        public void Send(TrackerEvent e)
        {
            eventsQueue.Add(e);

            if (eventsQueue.Count == maxSizeQueue)
            {
                Flush();
                eventsQueue.Clear();
            }

        }

        public void Flush()
        {
            foreach (var item in eventsQueue)
            {
                byte[] data = Encoding.UTF8.GetBytes(mySerializer.Serialize(item));
                fs.Write(data, 0, data.Length);
            }
        }
        void closeFile()
        {
            mySerializer.EndFile(fs);
            fs.Close();
        }

        public void EndSession()
        {
            Flush();
            closeFile();
        }
    }
}

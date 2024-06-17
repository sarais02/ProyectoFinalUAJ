using System;
using System.Collections.Generic;
using System.IO;

namespace TrackerG5
{
    internal class FilePersistence : IPersistence
    {
        List<TrackerEvent> eventsQueue = new List<TrackerEvent>();
        ISerializer mySerializer;
        StreamWriter writer;
        uint maxSizeQueue;

        public FilePersistence(ISerializer serializer, string route, uint maxSizeQueue)
        {
            this.maxSizeQueue = maxSizeQueue;
            mySerializer = serializer;

            createFileStream(route);
        }

        private bool createFileStream(string route)
        {
            try
            {
                string directory = Path.GetDirectoryName(route);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                writer = new StreamWriter(route);
                writer.Write(mySerializer.OpenFile());
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error en la creación del archivo", ex);
            }

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
            if (WriterClosed(writer))
                return;

            foreach (var item in eventsQueue)
            {
                writer.Write(mySerializer.Serialize(item));
            }
        }

        private bool WriterClosed(StreamWriter writer)
        {
            try
            {
                var stream = writer.BaseStream;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        void closeFile()
        {
            writer.Write(mySerializer.EndFile());
            writer.Close();
        }

        public void EndSession()
        {
            Flush();
            closeFile();
        }
    }
}

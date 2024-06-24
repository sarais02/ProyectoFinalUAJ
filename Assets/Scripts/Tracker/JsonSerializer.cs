using System.IO;
using System.Runtime.Serialization.Json;

namespace TrackerG5
{
    internal class JsonSerializer : ISerializer
    {

        private bool firstEvent = true;

        public string Serialize(TrackerEvent e)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonObj = new DataContractJsonSerializer(e.GetType());

            jsonObj.WriteObject(stream, e);
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(stream);

            if (!firstEvent)
                return "," + "\n" + streamReader.ReadToEnd();
            else
            {
                firstEvent = false;
                return "\n" + streamReader.ReadToEnd();
            }

        }

        public string OpenText()
        {
            return "[";
        }

        public string EndText()
        {
            return "]";
        }
    }
}

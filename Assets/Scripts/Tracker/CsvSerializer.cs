using System.Reflection;

namespace TrackerG5
{
    internal class CsvSerializer : ISerializer
    {
        public string Serialize(TrackerEvent e)
        {
            string csvProperties = "";

            PropertyInfo[] properties = e.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                csvProperties += property.GetValue(e) + ",";
            }
            csvProperties += e.GetType().Name + '\n';
            return csvProperties;
        }
        public string OpenText()
        {
            string csvProperties = "";

            PropertyInfo[] properties = new TrackerEvent().GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                csvProperties += property.Name + ",";
            }

            csvProperties += "EventType\n";
            return csvProperties;
        }

        public string EndText()
        {
            return " ";
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using System.Text;


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
        public void OpenFile(FileStream fs)
        {
            if (fs.Length <= 0)
            {
                fs.Seek(0, SeekOrigin.Begin);
                string csvProperties = "";

                PropertyInfo[] properties = new TrackerEvent().GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    csvProperties += property.Name+",";
                }
                
                csvProperties += "EventType\n";

                byte[] data = Encoding.UTF8.GetBytes(csvProperties);
                fs.Write(data, 0, data.Length);
            }
            
            fs.Seek(0, SeekOrigin.End);
        }

        public void EndFile(FileStream fs)
        {
        }
    }
}

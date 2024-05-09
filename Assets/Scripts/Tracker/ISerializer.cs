using System.IO;

namespace TrackerG5
{
    internal interface ISerializer
    {
        string Serialize(TrackerEvent e);

        void OpenFile(FileStream fs);

        void EndFile(FileStream fs);
    }
}

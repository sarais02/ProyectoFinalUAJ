
namespace TrackerG5
{
    internal interface ISerializer
    {
        string Serialize(TrackerEvent e);

        string OpenFile();

        string EndFile();
    }
}

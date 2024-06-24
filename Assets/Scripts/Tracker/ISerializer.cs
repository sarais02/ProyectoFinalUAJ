
namespace TrackerG5
{
    internal interface ISerializer
    {
        string Serialize(TrackerEvent e);

        string OpenText();

        string EndText();
    }
}

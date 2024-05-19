
namespace TrackerG5
{
    internal interface IPersistence
    {
        void Send(TrackerEvent e);

        void Flush();

        void EndSession();
    }
}

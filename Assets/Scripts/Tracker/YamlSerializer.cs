using System.Linq;
using System.Reflection;
using System.Text;
using TrackerG5;

namespace Assets.Scripts.TRACKERG5
{
    internal class YamlSerializer : ISerializer
    {
        string old_IdUser = "";
        string old_IdSession = "";

        public string Serialize(TrackerEvent e)
        {
            StringBuilder csvBuilder = new StringBuilder();
            PropertyInfo[] properties = e.GetType().GetProperties();

            var otherProp = properties.Where(p => p.Name != "IdUser" && p.Name != "IdSession" && p.Name != "Id").ToArray();
            properties = properties
                .Where(p => p.Name == "IdUser" || p.Name == "IdSession" || p.Name == "Id")
                .Concat(otherProp)
                .ToArray();

            //ID DE USUARIO
            if (old_IdUser != e.IdUser)
            { 
                csvBuilder.AppendLine($"- IdUser: {e.IdUser}");
                old_IdUser = e.IdUser;
            }

            //ID DE SESSION
            if (old_IdSession != e.IdSession) 
            {
                csvBuilder.AppendLine($" - IdSession: {e.IdSession}");
                old_IdSession = e.IdSession;
                csvBuilder.AppendLine("   Events:");
            }

            //id del evento
            csvBuilder.AppendLine($"     - Id: {e.Id}");
            //tipo de evento
            csvBuilder.AppendLine($"       TypeEvent: {e.GetType().Name}");

            foreach (PropertyInfo property in otherProp)
            {
                csvBuilder.AppendLine($"       {property.Name}: {property.GetValue(e)}");
            }

            return csvBuilder.ToString();
        }
        public string OpenText()
        {
            return " ";
        }

        public string EndText()
        {
            return " ";
        }
    }
}

using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TrackerG5;

namespace Assets.Scripts.TRACKERG5
{
    internal class YamlSerializer: ISerializer
    {
        string old_IdUser = "";
        string old_IdSession = "";

        public string Serialize(TrackerEvent e)
        {
            StringBuilder csvBuilder = new StringBuilder();

            PropertyInfo[] properties = e.GetType().GetProperties();
            
            // Obtener todas las propiedades excepto IdUsuario e IdSession
            var otherProp = properties.Where(p => p.Name != "IdUser" && p.Name != "IdSession" && p.Name != "Id").ToArray();

            // Ordenar el arreglo de propiedades
            properties = properties
                .Where(p => p.Name == "IdUser" || p.Name == "IdSession" || p.Name == "Id") // Seleccionar IdUsuario e IdSession primero
                .Concat(otherProp) // Concatenar el resto de las propiedades
                .ToArray();

            if (old_IdUser != e.IdUser) { //ID DE USUARIO
                csvBuilder.AppendLine($"- IdUser: {e.IdUser}");
                old_IdUser = e.IdUser;

                csvBuilder.AppendLine("  Sessions:");
            }

            if(old_IdSession != e.IdSession) //ID DE SESSION
            {
                csvBuilder.AppendLine($"  - IdSession: {e.IdSession}");
                old_IdSession= e.IdSession;
                csvBuilder.AppendLine("    Events:");
            }

            //id del evento
            csvBuilder.AppendLine($"      - Id: {e.Id}");

            //tipo de evento
            csvBuilder.AppendLine($"        TypeEvent: {e.GetType().Name}");

            //timespan idlevel + cosas del evento
            foreach (PropertyInfo property in otherProp)
            {
                csvBuilder.AppendLine($"        {property.Name}: {property.GetValue(e)}");
            }

            return csvBuilder.ToString();
        }
        public void OpenFile(FileStream fs)
        {
            fs.Seek(0, SeekOrigin.End);
        }

        public void EndFile(FileStream fs)
        {
        }
    }
}

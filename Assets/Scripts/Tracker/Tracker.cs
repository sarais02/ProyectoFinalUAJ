using Assets.Scripts.TRACKERG5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TrackerG5
{
    class Tracker
    {
        private static Tracker instance;
        string idUser;
        string idSession;
        string idUserNameLocation = "../Assets/Scripts/Tracker/Data";
        string resultLocation = "";

        const int size = 7;

        IPersistence persistence;
        ISerializer serializer;
        HashSet<ITrackerAsset> assets = new HashSet<ITrackerAsset>();//lista de assets

        public enum serializeType { Json, Csv, Yaml };
        public enum persistenceType { Disc };
        public enum eventType { BotPosition, StartTest, EndTest };




        Tracker() { }
        public static Tracker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Tracker();
                }
                return instance;
            }
        }

        private string GetUserID()
        {
            if (!File.Exists(idUserNameLocation))
            {
                File.WriteAllText(idUserNameLocation, CreateHashID(DateTime.Now.ToString() + new Random().Next()));
            }

            return File.ReadAllText(idUserNameLocation);
        }


        public void AddEvent(eventType eventT, Dictionary<string, string> eventParameters = null)
        {

            TrackerEvent e = null;
            switch (eventT)
            {
                case eventType.BotPosition:
                    e = new BotPositionEvent();
                    break;
                case eventType.StartTest:
                    e = new StartTestEvent();

                    break;
                case eventType.EndTest:
                    e = new EndTestEvent();

                    break;
                default:
                    break;
            }

            e.Id = CreateHashID(idUser + DateTime.Now.ToString());
            e.IdUser = idUser;
            e.IdSession = idSession;
            e.Timestamp = DateTime.Now;

            if (e.SetParamns(eventParameters))
                persistence.Send(e);

        }

        public void Init(serializeType sT, persistenceType pT)
        {
            idUser = GetUserID();
            idSession = CreateHashID(idUser + DateTime.Now.ToString() + "tracker");

            Console.WriteLine("USER ID: " + idUser + " SESSION ID: " + idSession);
            //evento de inicio de sesion

            switch (sT)
            {
                case serializeType.Json:
                    serializer = new JsonSerializer();
                    resultLocation = "../Tracking System/Assets/Scripts/TRACKERG5/Data/RESULT.json";
                    break;
                case serializeType.Csv:
                    serializer = new CsvSerializer();
                    resultLocation = "../Tracking System/Assets/Scripts/TRACKERG5/Data/RESULT.csv";
                    break;
                case serializeType.Yaml:
                    serializer = new YamlSerializer();
                    resultLocation = "../Tracking System/Assets/Scripts/TRACKERG5/Data/RESULT.yaml";
                    break;
                default:
                    throw new Exception("Serializacion no valida");
            }

            switch (pT)
            {
                case persistenceType.Disc:
                    persistence = new FilePersistence(serializer, resultLocation, size);
                    break;
                default:
                    throw new Exception("Persistencia no valida");

            };

            LoginEvent e = new LoginEvent();
            e.Id = CreateHashID(idUser + DateTime.Now.ToString());
            e.IdUser = idUser;
            e.IdSession = idSession;
            e.Timestamp = DateTime.Now;

            persistence.Send(e);

        }

        public void End()
        {
            //evento de fin de inicio de sesion
            LogoutEvent e = new LogoutEvent();
            e.Id = CreateHashID(idUser + DateTime.Now.ToString());
            e.IdUser = idUser;
            e.IdSession = idSession;
            e.Timestamp = DateTime.Now;

            persistence.Send(e);

            persistence.EndSession();
        }

        private string CreateHashID(string blockchain)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(blockchain);
            bytes = sha256.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}

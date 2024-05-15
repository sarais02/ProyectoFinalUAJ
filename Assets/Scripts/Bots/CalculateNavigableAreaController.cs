using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackingBots
{

    public class CalculateNavigableAreaController : MonoBehaviour
    {
        //variables generales
        public uint nBots;

        public Mesh visualBot;

        public Material visualBotMaterial;

        public Transform spawnPoint;

        public LayerMask terrainMask;

        public float maxDispersionBots, maxHeightOfTheMap;

        //variables relacionadas con el tipo de movimiento
        public MoveType moveType;

        public float distanceMove, jumpingForce;

        public enum MoveType { normal, jumping }


        //vairables internas
        Transform botsParent;

        Dictionary<string, string> eventParams = new Dictionary<string, string>();

        public bool testEnable = false;
        public void StartTest()
        {
            TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
            eventParams.Add("nBots", nBots.ToString());
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
            testEnable = true;
            Debug.Log("Test iniciado");
        }
        public void EndTest()
        {
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
            TrackerG5.Tracker.Instance.End();
            testEnable = false;
            Debug.Log("Test finalizado");
        }

        public void GenerateBots()
        {
            if (transform.childCount != 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }

            }

            botsParent = new GameObject("BotsRoot").GetComponent<Transform>();
            botsParent.parent = transform;

            SpawnBots();

        }

        void SpawnBots()
        {
            for (int i = 0; i < nBots; i++)
            {
                Vector3 positionToSpawn;
                RaycastHit hit;
                do
                {
                    Physics.Raycast(new Vector3(spawnPoint.position.x + Random.Range(-maxDispersionBots/4, maxDispersionBots/4), maxHeightOfTheMap, spawnPoint.position.z + Random.Range(-maxDispersionBots / 4, maxDispersionBots / 4)),
                        Vector3.down, out hit, Mathf.Infinity, terrainMask);
                } while (hit.collider == null);

                positionToSpawn = hit.point;
                GenerateBot("Bot" + i.ToString(), positionToSpawn);
            }
        }

        GameObject GenerateBot(string name, Vector3 position)
        {
            GameObject gO = new GameObject(name, typeof(BotMovement), typeof(Rigidbody), typeof(MeshRenderer), typeof(BotTracker));
            gO.transform.parent = botsParent;
            gO.transform.position = position;
            return gO;
        }
    }
}
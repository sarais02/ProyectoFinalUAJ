using System;
using System.Collections;
using System.Collections.Generic;
using TrackerG5;
using UnityEngine;

namespace TrackingBots
{

    public class CalculateNavigableAreaController : MonoBehaviour
    {
        //variables generales
        public uint nBots;

        [SerializeField] GameObject visualBot;

        [SerializeField] Transform spawnPoint;

        [SerializeField] LayerMask terrainMask;

        [SerializeField] float maxDispersionBots, maxHeightOfTheMap;

        [SerializeField] MapGenerator mapAssociated;
        //variables relacionadas con el tipo de movimiento
        public MoveType moveType;

        [SerializeField] float distanceMove, jumpingForce;

       public enum MoveType { normal, jumping }

        //para el movimiento normal
        [SerializeField] float wanderRadius;
        [SerializeField][Range(1, 3)] float wanderRandomRelative;
        [SerializeField] PhysicMaterial colliderMat;


        //vairables internas
        Transform botsParent;

        Dictionary<string, string> eventParams = new Dictionary<string, string>();

        bool testEnable = false;

        public bool TestEnable{ get { return testEnable; } }

       
        public void StartTest()
        {
            if (testEnable)
                return;

            eventParams.Clear();

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

        public void SessionInfo(out CalculateNavigableAreaController controller, out MapGenerator map)
        {
            controller = this;
            map = mapAssociated;
        }

        void SpawnBots()
        {
            for (int i = 0; i < nBots; i++)
            {
                Vector3 positionToSpawn;
                RaycastHit hit;
                do
                {
                    Physics.Raycast(new Vector3(spawnPoint.position.x + UnityEngine.Random.Range(-maxDispersionBots / 4, maxDispersionBots / 4), maxHeightOfTheMap, spawnPoint.position.z + UnityEngine.Random.Range(-maxDispersionBots / 4, maxDispersionBots / 4)),
                        Vector3.down, out hit, Mathf.Infinity, terrainMask);
                } while (hit.collider == null);

                positionToSpawn = hit.point;
                GenerateBot("Bot" + i.ToString(), positionToSpawn);
            }
        }

        GameObject GenerateBot(string name, Vector3 position)
        {
            GameObject gO = new GameObject(name, typeof(BotTracker), typeof(WanderBot), typeof(Rigidbody), typeof(SphereCollider));

            var bBody = gO.GetComponent<Rigidbody>();
            bBody.useGravity = false;
            bBody.constraints = RigidbodyConstraints.FreezeRotation;
            var bColl = gO.GetComponent<SphereCollider>();
            bColl.material = colliderMat;
           

            if (visualBot != null)
            {
                Instantiate<GameObject>(visualBot, gO.transform);
            }


            //modificaciones para normal o salto
            gO.GetComponent<WanderBot>().SetParams(wanderRadius, wanderRandomRelative, this, mapAssociated);

          

            gO.transform.parent = botsParent;
            gO.transform.position = position + Vector3.up*0.5f;
            return gO;
        }
    }
}
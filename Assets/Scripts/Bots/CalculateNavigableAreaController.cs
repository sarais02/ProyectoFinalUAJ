using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;


namespace TrackingBots
{
    [Serializable]
    public class Config
    {
        public uint nBots;
        public uint maxTimeTest;
        public float maxDispersionBots, maxHeightOfTheMap;
        public float wanderRadius;
        public float scaleTimeInTest;
    }
    public class CalculateNavigableAreaController : MonoBehaviour
    {
        //variables generales
        public uint nBots;

        [SerializeField] GameObject visualBot;

        [SerializeField] Transform spawnPoint;

        [SerializeField] LayerMask terrainMask;

        [SerializeField] float maxDispersionBots, maxHeightOfTheMap;
        //variables relacionadas con el tipo de movimiento

        [SerializeField] float distanceMove;

        //para el movimiento normal
        [SerializeField] float wanderRadius;
        [SerializeField][Range(1, 3)] float wanderRandomRelative;
        [SerializeField] PhysicMaterial colliderMat;


        //vairables internas
        Transform botsParent;

        Dictionary<string, string> eventParams = new Dictionary<string, string>();

        [SerializeField] bool testEnable = false;

        public bool TestEnable { get { return testEnable; } }
        public LayerMask TerrainMask { get { return terrainMask; } }
        public float MaxHeightOfTheMap { get { return maxHeightOfTheMap; } }

        [SerializeField][Range(0.5f, 5)] float scaleTimeInTest = 1;
        [SerializeField] uint maxTimeTest = 3600;

        [SerializeField] List<Transform> bots;


        int areasAchieve = 0;
        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            LoadParameters();
            
#endif
          
        }
        private void Start()
        {
#if !UNITY_EDITOR
            //Si es una build genero bots automaticamente y empiezo el test
            GenerateBots();
            StartTest();
            CallTrackerStart();
            
#endif
            scaleTimeInTest = Math.Max(scaleTimeInTest, 0.1f);
            Time.timeScale = scaleTimeInTest;

            Invoke("EndTestByTime", maxTimeTest / scaleTimeInTest);
        }
        void LoadParameters()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "config.json");
            string jsonContent = File.ReadAllText(filePath);

            Config config = new Config();
            config = JsonUtility.FromJson<Config>(jsonContent);
            nBots = config.nBots;
            maxDispersionBots = config.maxDispersionBots;
            maxHeightOfTheMap = config.maxHeightOfTheMap;
            maxTimeTest = config.maxTimeTest;
            wanderRadius = config.wanderRadius;
            scaleTimeInTest = config.scaleTimeInTest;
            if (scaleTimeInTest > 5f) scaleTimeInTest = 5f;
            else if (scaleTimeInTest < 0.5f) scaleTimeInTest = 0.5f;
        }
        void CallTrackerStart()
        {
            TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
            eventParams.Add("nBots", nBots.ToString());
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
            Debug.Log("Test iniciado");
        }

        void CallTrackerEnd()
        {

            eventParams.Clear();

            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
            TrackerG5.Tracker.Instance.End();
            Debug.Log("Test finalizado");

        }
        private void OnDisable()
        {
            CancelInvoke();
        }

        void EndTestByTime()
        {  
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (testEnable)
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    eventParams.Clear();

                    TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
                    eventParams.Add("nBots", nBots.ToString());
                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
                    Debug.Log("Test iniciado");
                }
                else if (state == PlayModeStateChange.ExitingPlayMode)
                {

                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
                    TrackerG5.Tracker.Instance.End();
                    Debug.Log("Test finalizado");
                }
            }
        }
#endif
        public void StartTest()
        {
            testEnable = true;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

        }

        public void EndTest()
        {
            testEnable = false;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
            TrackerG5.Tracker.Instance.End();
#endif
            EndTest();
            CancelInvoke();
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

        public void SessionInfo(out CalculateNavigableAreaController controller)
        {
            controller = this;
        }

        void SpawnBots()
        {
            bots = new List<Transform>();

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

                bots.Add(GenerateBot("Bot" + i.ToString(), positionToSpawn).transform);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        GameObject GenerateBot(string name, Vector3 position)
        {
            GameObject gO = new GameObject(name, typeof(BotTracker), typeof(WanderBot), typeof(Rigidbody), typeof(SphereCollider));

            var bBody = gO.GetComponent<Rigidbody>();
            bBody.useGravity = false;
            bBody.constraints = RigidbodyConstraints.FreezeRotationY;
            var bColl = gO.GetComponent<SphereCollider>();
            bColl.material = colliderMat;


            if (visualBot != null)
            {
                Instantiate<GameObject>(visualBot, gO.transform);
            }


            //modificaciones para normal o salto
            gO.GetComponent<WanderBot>().SetParams(wanderRadius, wanderRandomRelative, this);
            var bTracker = gO.GetComponent<BotTracker>();
            bTracker.Controller = this;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(bTracker);
#endif

            gO.transform.parent = botsParent;
            gO.transform.position = position + Vector3.up * 0.5f;
            return gO;
        }
    }
}
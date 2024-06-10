using System;
using System.Collections;
using System.Collections.Generic;
using TrackerG5;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Playables;

namespace TrackingBots
{
    [Serializable]
    public class Config
    {
        public uint nBots;
        public uint maxTimeTest;
        public float maxDispersionBots, maxHeightOfTheMap;
        public float wanderRadius;
        public float mapSize;
        public float timeCheck;
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
        //[SerializeField] TextAsset json;

        public bool TestEnable{ get { return testEnable; } }
        public LayerMask TerrainMask { get { return terrainMask; } }
        public float MaxHeightOfTheMap { get { return maxHeightOfTheMap; } }

        [SerializeField] float mapSize = 10;
        [SerializeField][Range(1, 10)] int precisionLevel = 1;
        [SerializeField] float timeCheck = 1;

        [SerializeField][Range(0.5f,5)] float scaleTimeInTest = 1;
        [SerializeField] uint maxTimeTest = 3600;

        [SerializeField] List<Transform> bots;

        //List<AchievableGrid> achievableGrid;


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

            //CreateGridMap();

            Time.timeScale = scaleTimeInTest;

            InvokeRepeating("ActualiceGrid", 0.5f, timeCheck);
            Invoke("EndTestByTime", maxTimeTest);
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
            mapSize = config.mapSize;
            wanderRadius = config.wanderRadius;
            timeCheck = config.timeCheck;
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
            //eventParams.Add("mapAchieve", ((areasAchieve / (float)achievableGrid.Count) * 100).ToString());

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
            //para editor
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
            //para ejecutable
        }

//        void ActualiceGrid()
//        {
//            foreach (var grid in achievableGrid)
//            {
//                if (!grid.Achieve)
//                {
//                    if (grid.CheckBots(bots))
//                    {
//                        areasAchieve++;
                        
//                        //todo recorrido
//                        if(areasAchieve == achievableGrid.Count)
//                        {
//#if UNITY_EDITOR
//                            if (Application.isEditor)
//                            {
//                                UnityEditor.EditorApplication.isPlaying = false;
//                            }
//#endif
//                        }
//                    }
//                }
//            }
//        }

        private void Update()
        {
            //LoadParameters();
        }
        private void OnDestroy()
        {
            CancelInvoke();
        }
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(testEnable)
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

                    eventParams.Clear();
                    //eventParams.Add("mapAchieve", ((areasAchieve / (float)achievableGrid.Count) * 100).ToString());

                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest, eventParams);
                    TrackerG5.Tracker.Instance.End();
                    Debug.Log("Test finalizado");

                   
                    //Debug.Log("Porcentaje de mapa alcanzado: " + (areasAchieve / (float)achievableGrid.Count) * 100 + "%");
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
            EndTest();
            CancelInvoke();
        }

        public void GenerateBots()
        {
//#if UNITY_EDITOR
//            LoadParameters();
//#endif
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

        //void CreateGridMap()
        //{
        //    int numGrids = (9 + precisionLevel);
        //    float sizeGrid = mapSize / numGrids;

        //    achievableGrid = new List<AchievableGrid>();

        //    float x, z;
        //    x = spawnPoint.position.x - mapSize / 2 + sizeGrid / 2;
        //    z = spawnPoint.position.z - mapSize / 2 + sizeGrid / 2;

        //    // j = x i = z
        //    for (int i = 0; i < numGrids; i++)
        //    {
        //        for (int j = 0; j < numGrids; j++)
        //        {
        //            if (Physics.CheckBox(new Vector3(x, 0, z), new Vector3(sizeGrid / 2, maxHeightOfTheMap, sizeGrid / 2), Quaternion.identity, terrainMask))
        //            {
        //                AchievableGrid grid = new AchievableGrid();
        //                grid.SetParams(x - sizeGrid / 2, x + sizeGrid / 2, z - sizeGrid / 2, z + sizeGrid / 2);
        //                achievableGrid.Add(grid);
        //            }

        //            x += sizeGrid;
        //        }

        //        x = spawnPoint.position.x - mapSize / 2 + sizeGrid / 2;
        //        z += sizeGrid;
        //    }

        //}

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
            gO.transform.position = position + Vector3.up*0.5f;
            return gO;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TrackerG5;
using UnityEditor;
using UnityEngine;

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
        private uint nBots_;

        [SerializeField] GameObject visualBot;

        [SerializeField] Transform spawnPoint;

        [SerializeField] LayerMask terrainMask;

        [SerializeField] float maxDispersionBots, maxHeightOfTheMap;
        private float maxDispersionBots_, maxHeightOfTheMap_;
        //variables relacionadas con el tipo de movimiento
        public MoveType moveType;

        [SerializeField] float distanceMove, jumpingForce;

       public enum MoveType { normal, jumping }

        //para el movimiento normal
        [SerializeField] float wanderRadius;
        private float wanderRadius_;
        [SerializeField][Range(1, 3)] float wanderRandomRelative;
        [SerializeField] PhysicMaterial colliderMat;


        //vairables internas
        Transform botsParent;

        Dictionary<string, string> eventParams = new Dictionary<string, string>();

        [SerializeField] bool testEnable = false;
        [SerializeField] TextAsset json;

        public bool TestEnable{ get { return testEnable; } }
        public LayerMask TerrainMask { get { return terrainMask; } }
        public float MaxHeightOfTheMap { get { return maxHeightOfTheMap_; } }

        [SerializeField] float mapSize = 10;
        private float mapSize_;
        [SerializeField][Range(1, 10)] int precisionLevel = 1;
        [SerializeField] float timeCheck = 1;
        private float timeCheck_;

        [SerializeField][Range(0.5f,5)] float scaleTimeInTest = 1;
        [SerializeField] uint maxTimeTest = 3600;
        private uint maxTimeTest_;

        [SerializeField] List<Transform> bots;

        List<AchievableGrid> achievableGrid;


        int areasAchieve = 0;
        private void Awake()
        {
#if UNITY_EDITOR
            // En el editor, usar los valores del script
            nBots_ = nBots;
            maxDispersionBots_ = maxDispersionBots;
            maxHeightOfTheMap_ = maxHeightOfTheMap;
            maxTimeTest_ = maxTimeTest;
            mapSize_ = mapSize;
            wanderRadius_ = wanderRadius;
            timeCheck_ = timeCheck;
#else
            //En una build, cargar los valores desde un archivo JSON
            Config config = new Config();
            config = JsonUtility.FromJson<Config>(json.text);
            nBots_ = config.nBots;
            maxDispersionBots_ = config.maxDispersionBots;
            maxHeightOfTheMap_ = config.maxHeightOfTheMap;
            maxTimeTest_ = config.maxTimeTest;
            mapSize_ = config.mapSize;
            wanderRadius_ = config.wanderRadius;
            timeCheck_ = config.timeCheck;
#endif

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;            
        }
        private void Start()
        {
            CreateGridMap();

            Time.timeScale = scaleTimeInTest;

            InvokeRepeating("ActualiceGrid", 0.5f, timeCheck_);
            Invoke("EndTestByTime", maxTimeTest_);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        void EndTestByTime()
        {
            //para editor
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            //para ejecutable
        }

        void ActualiceGrid()
        {
            foreach (var grid in achievableGrid)
            {
                if (!grid.Achieve)
                {
                    if (grid.CheckBots(bots))
                    {
                        areasAchieve++;
                        
                        //todo recorrido
                        if(areasAchieve == achievableGrid.Count)
                        {
                            if(Application.isEditor)
                            {
                                UnityEditor.EditorApplication.isPlaying = false;
                            }
                        }
                    }
                }
            }
        }

        
        private void OnDestroy()
        {
            CancelInvoke();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(testEnable)
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    eventParams.Clear();

                    TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
                    eventParams.Add("nBots", nBots_.ToString());
                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
                    Debug.Log("Test iniciado");
                }
                else if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
                    TrackerG5.Tracker.Instance.End();
                    Debug.Log("Test finalizado");

                   
                    Debug.Log("Porcentaje de mapa alcanzado: " + (areasAchieve / (float)achievableGrid.Count) * 100 + "%");
                }
            }
          
        }

        public void StartTest()
        {
            testEnable = true;
            UnityEditor.EditorUtility.SetDirty(this);

        }

        public void EndTest()
        {
            testEnable = false;
            UnityEditor.EditorUtility.SetDirty(this);
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

        void CreateGridMap()
        {
            int numGrids = (9 + precisionLevel);
            float sizeGrid = mapSize_ / numGrids;

            achievableGrid = new List<AchievableGrid>();

            float x, z;
            x = spawnPoint.position.x - mapSize_ / 2 + sizeGrid / 2;
            z = spawnPoint.position.z - mapSize_ / 2 + sizeGrid / 2;

            // j = x i = z
            for (int i = 0; i < numGrids; i++)
            {
                for (int j = 0; j < numGrids; j++)
                {
                    if (Physics.CheckBox(new Vector3(x, 0, z), new Vector3(sizeGrid / 2, maxHeightOfTheMap_, sizeGrid / 2), Quaternion.identity, terrainMask))
                    {
                        AchievableGrid grid = new AchievableGrid();
                        grid.SetParams(x - sizeGrid / 2, x + sizeGrid / 2, z - sizeGrid / 2, z + sizeGrid / 2);
                        achievableGrid.Add(grid);
                    }

                    x += sizeGrid;
                }

                x = spawnPoint.position.x - mapSize_ / 2 + sizeGrid / 2;
                z += sizeGrid;
            }

        }

        public void SessionInfo(out CalculateNavigableAreaController controller)
        {
            controller = this;
        }

        void SpawnBots()
        {
            bots = new List<Transform>();

            for (int i = 0; i < nBots_; i++)
            {
                Vector3 positionToSpawn;
                RaycastHit hit;
                do
                {
                    Physics.Raycast(new Vector3(spawnPoint.position.x + UnityEngine.Random.Range(-maxDispersionBots_ / 4, maxDispersionBots_ / 4), maxHeightOfTheMap_, spawnPoint.position.z + UnityEngine.Random.Range(-maxDispersionBots_ / 4, maxDispersionBots_ / 4)),
                        Vector3.down, out hit, Mathf.Infinity, terrainMask);
                } while (hit.collider == null);

                positionToSpawn = hit.point;

                bots.Add(GenerateBot("Bot" + i.ToString(), positionToSpawn).transform);
            }


            UnityEditor.EditorUtility.SetDirty(this);
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
            gO.GetComponent<WanderBot>().SetParams(wanderRadius_, wanderRandomRelative, this);
            var bTracker = gO.GetComponent<BotTracker>();
            bTracker.Controller = this;
            UnityEditor.EditorUtility.SetDirty(bTracker);


            gO.transform.parent = botsParent;
            gO.transform.position = position + Vector3.up*0.5f;
            return gO;
        }
    }
}
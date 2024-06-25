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


        //variables internas
        Transform botsParent;

        Dictionary<string, string> eventParams = new Dictionary<string, string>();

        [SerializeField] bool testEnable = false;

        public bool TestEnable { get { return testEnable; } }
        public LayerMask TerrainMask { get { return terrainMask; } }
        public float MaxHeightOfTheMap { get { return maxHeightOfTheMap; } }

        [SerializeField][Range(0.5f, 5)] float scaleTimeInTest = 1;
        [SerializeField] uint timeTest = 3600;

        [SerializeField] List<Transform> bots;

        const float minTime = 0.5f, maxTime = 5f;

        private void Awake()
        {
#if UNITY_EDITOR
            //Si se esta en el editor se suscribe al evento de cambio de modo de juego para crear y cerrar los tests
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            //Si se esta en la build carga los parametros para crear el test
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

            //Se ajusta la escala de tiempo acorde a los valores del usuario y siempre validos
            AdjustTime(ref scaleTimeInTest);
            Time.timeScale = scaleTimeInTest;

            //Se llama a finalizar el test dado el tiempo por el usuario y teniendo en cuenta la escala de tiempo 
            Invoke("EndTestByTime", timeTest);
        }

        //Ajusta el tiempo para que quede dentro d elos valores maximos y minimos
        void AdjustTime(ref float targetTime)
        {
            if (targetTime < minTime)
            {
                targetTime = minTime;
            }
            else if (targetTime > maxTime)
            {
                targetTime = maxTime;
            }
        }
        void LoadParameters()
        {
            //Se obtienen los valores del test a tráves del Json config
            string filePath = Path.Combine(Application.streamingAssetsPath, "config.json");
            string jsonContent = File.ReadAllText(filePath);
            Config config = new Config();
            config = JsonUtility.FromJson<Config>(jsonContent);
            nBots = config.nBots;
            maxDispersionBots = config.maxDispersionBots;
            maxHeightOfTheMap = config.maxHeightOfTheMap;
            timeTest = config.maxTimeTest;
            wanderRadius = config.wanderRadius;
            scaleTimeInTest = config.scaleTimeInTest;
          
        }
        void CallTrackerStart()
        {
            //inicializa el tracker
            TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
            eventParams.Add("nBots", nBots.ToString());
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
            Debug.Log("Test iniciado");
        }

        void CallTrackerEnd()
        {
            //Cierra el tracker y envia el evento de fin de sesión
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
            Debug.Log("tiempo: " + Time.timeSinceLevelLoad);
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
            //si los test estan activos
            if (testEnable)
            {
                //crea un test al iniciar al entrar en modo "play"
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    eventParams.Clear();

                    TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
                    eventParams.Add("nBots", nBots.ToString());
                    TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
                    Debug.Log("Test iniciado");
                }
                //y lo cierra al salir del modo "play"
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
            //Realziamos las llamadas pertientes segun si se esta en el ejecutable o el editor
#if !UNITY_EDITOR
            //Si es la build añade el evento de fin de test y solicita al tracker que se cierra
            TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
            TrackerG5.Tracker.Instance.End();
#endif
            //Si es el editor llama al método pertinente 
            EndTest();
            CancelInvoke();
        }

        public void GenerateBots()
        {
            //Elimina los bots existentes
            if (transform.childCount != 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }

            }

            //Crea un objeto para almacenar los nuevos bots y los genera
            botsParent = new GameObject("BotsRoot").GetComponent<Transform>();
            botsParent.parent = transform;
            SpawnBots();

        }

        void SpawnBots()
        {
            bots = new List<Transform>();

            for (int i = 0; i < nBots; i++)
            {
                //busca una posición libre para los bots mediante raycast
                Vector3 positionToSpawn;
                RaycastHit hit;
                do
                {
                    Physics.Raycast(new Vector3(spawnPoint.position.x + UnityEngine.Random.Range(-maxDispersionBots / 4, maxDispersionBots / 4), maxHeightOfTheMap, spawnPoint.position.z + UnityEngine.Random.Range(-maxDispersionBots / 4, maxDispersionBots / 4)),
                        Vector3.down, out hit, Mathf.Infinity, terrainMask);
                } while (hit.collider == null);

                positionToSpawn = hit.point;

                //y lo genera
                bots.Add(GenerateBot("Bot" + i.ToString(), positionToSpawn).transform);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        GameObject GenerateBot(string name, Vector3 position)
        {

            //Se le asignan los componentes necesarios
            GameObject gO = new GameObject(name, typeof(BotTracker), typeof(WanderBot), typeof(Rigidbody), typeof(SphereCollider));

            var bBody = gO.GetComponent<Rigidbody>();
            bBody.useGravity = false;
            bBody.constraints = RigidbodyConstraints.FreezeRotationY;
            var bColl = gO.GetComponent<SphereCollider>();
            bColl.material = colliderMat;

            //Se le crea la parte visual si la tiene
            if (visualBot != null)
            {
                Instantiate<GameObject>(visualBot, gO.transform);
            }

           // Y el script de comportamiento con los parametros dados por el usuario
            gO.GetComponent<WanderBot>().SetParams(wanderRadius, wanderRandomRelative, this);
            var bTracker = gO.GetComponent<BotTracker>();
            bTracker.Controller = this;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(bTracker);
#endif
            //Y lo situa en el mapa
            gO.transform.parent = botsParent;
            gO.transform.position = position + Vector3.up * 0.5f;
            return gO;
        }
    }
}
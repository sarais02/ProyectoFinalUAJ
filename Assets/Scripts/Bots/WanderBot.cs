using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.TerrainUtils;

namespace TrackingBots
{
    public class WanderBot : MonoBehaviour
    {
        [SerializeField] MapGenerator mapGenerator;

        [Header("Wander")]
        [SerializeField] float wanderRadius = 50f;
        [SerializeField] float targetYOffset = 0.5f;
        [SerializeField][Range(1, 3)] float wanderRandomRelative = 5;

        [Header("Other Forces")]
        [SerializeField] float gravity = 15f;
        Vector3 gravV3;

        [Header("Debug Help")]
        [SerializeField] GameObject beaconPrefab;

        Vector3 currTargetPos;
        Vector3 lastTargetPos;
        GameObject aux;
        Rigidbody rb;

        float movSpeed;

        [SerializeField] CalculateNavigableAreaController controller;


        float minDistInOneSec = 2f;
        float distCheckRefreshSecs = 1f;
        Vector3 lastPosCheck;

        static uint maxSearch = 100;
        private void Awake()
        {
            movSpeed = Mathf.Max(wanderRadius / (wanderRandomRelative * 3), 1f);
        }

        void Start()
        {
            if (!controller.TestEnable)
                return;

            rb = GetComponent<Rigidbody>();
            gravV3 = new Vector3(0f, -gravity, 0f);

            InvokeRepeating(nameof(GenerateNewPosition), 0f, 4f);



        }


        private void FixedUpdate()
        {
            if (!controller.TestEnable)
                return;

            Vector2 distance = new Vector2(currTargetPos.x - transform.position.x, currTargetPos.z - transform.position.z);
            Vector2 targetVel = distance.normalized * movSpeed;
            Vector2 steering = new Vector2(targetVel.x - rb.velocity.x, targetVel.y - rb.velocity.z);

            rb.AddForce(steering.x, 0f, steering.y, ForceMode.Force);
            rb.AddForce(gravV3, ForceMode.Acceleration);

            CheckPosition();
        }

        public void SetParams(float wanderRadius, float wanderRelative, CalculateNavigableAreaController controller,
            MapGenerator map)
        {

            this.wanderRadius = wanderRadius;
            wanderRandomRelative = wanderRelative;
            this.controller = controller;
            mapGenerator = map;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        void CheckPosition()
        {
            FallingBotRes();

            if (Vector3.Distance(transform.position, currTargetPos) < 2f)
            {
                Debug.Log("LLEGADO A DESTINO");
                ResetInvoke();
                GenerateNewPosition();
            }
        }

        void ResetInvoke()
        {
            CancelInvoke(nameof(GenerateNewPosition));
            InvokeRepeating(nameof(GenerateNewPosition), 0f, 4f);
        }

        void GenerateNewPosition()
        {

            lastTargetPos = transform.position;

            //codigo del mapa
            /*
            Vector2 pointOnMap = new Vector2(Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius),
                  Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius));

            currTargetPos = mapGenerator.GetGlobalPosition(pointOnMap);
            currTargetPos.y += targetYOffset;
            */
            currTargetPos = GetPos();
            Debug.Log(currTargetPos);

            if (beaconPrefab != null)
            {
                GameObject.Destroy(aux); //QUITAR
                aux = Instantiate(beaconPrefab, currTargetPos, Quaternion.identity); //QUITAR
            }

            CancelInvoke(nameof(CheckMovedInOneSec));
            InvokeRepeating(nameof(CheckMovedInOneSec), distCheckRefreshSecs, distCheckRefreshSecs);
        }

        RaycastHit hit;
        Vector3 GetPos()
        {
            int searchs = 0;
            do
            {
                Physics.Raycast(new Vector3(Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius), controller.MaxHeightOfTheMap,
                  Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius)), Vector3.down, out hit, Mathf.Infinity, controller.TerrainMask);
                searchs++;
            } while (hit.collider == null && searchs < maxSearch);

            //caso donde se ha quedado aislado desactivamos el bot
            if (searchs < maxSearch)
            {
                return hit.point;
            }
            else
            {
                Destroy(gameObject);
                CancelInvoke();
                return Vector3.zero;
            }


        }

        void FallingBotRes()
        {
            if (transform.position.y <= -10f)
            {
                transform.position = lastTargetPos;
                rb.velocity = Vector3.zero;
                Debug.Log("Reset Position by Falling out of the map");
            }
        }

        void CheckMovedInOneSec()
        {
            if (Vector3.Distance(transform.position, lastPosCheck) < minDistInOneSec)
            {
                Debug.Log("OBSTACULIZADO");
                ResetInvoke();
                GenerateNewPosition();
            }
            lastPosCheck = transform.position;
        }

    }
}
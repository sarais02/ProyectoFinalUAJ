using UnityEngine;

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

        static float movSpeed = 0f;

        private void Awake()
        {
            if (movSpeed == 0)
                movSpeed = Mathf.Max(wanderRadius / (wanderRandomRelative*4), 1f);

            Debug.Log(movSpeed);
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            gravV3 = new Vector3(0f, -gravity, 0f);

            InvokeRepeating(nameof(GenerateNewPosition), 0f, 4f);
        }

        private void FixedUpdate()
        {
            Vector2 distance = new Vector2(currTargetPos.x - transform.position.x, currTargetPos.z - transform.position.z);
            Vector2 targetVel = distance.normalized * movSpeed;
            Vector2 steering = new Vector2(targetVel.x - rb.velocity.x, targetVel.y - rb.velocity.z);

            rb.AddForce(steering.x, 0f, steering.y, ForceMode.Force);
            rb.AddForce(gravV3, ForceMode.Acceleration);

            CheckPosition();
        }

        public void SetParams(float wanderRadius, float wanderRelative)
        {
            this.wanderRadius = wanderRadius;
            wanderRandomRelative = wanderRelative;
        }

        void CheckPosition()
        {
            FallingBotRes();

            if (Vector3.Distance(transform.position, currTargetPos) < 2f)
            {
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

            Vector2 pointOnMap = new Vector2(Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius),
                  Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius));

            currTargetPos = mapGenerator.GetGlobalPosition(pointOnMap);
            currTargetPos.y += targetYOffset;

            if (beaconPrefab != null) {
                GameObject.Destroy(aux); //QUITAR
                aux = Instantiate(beaconPrefab, currTargetPos, Quaternion.identity); //QUITAR
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
    }
}
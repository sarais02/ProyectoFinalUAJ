using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackingBots
{
    public class BotMovement : MonoBehaviour
    {

        [SerializeField]
        MapGenerator mapGenerator;

        [SerializeField]
        float velocity = 5f;
        [SerializeField]
        float gravity = 10f;
        [SerializeField]
        public float movement_Radius = 50f;

        Vector3 movemntToPosition;
        Rigidbody rb;

        [SerializeField]
        float minDistInOneSec = 2f;

        float distCheckRefreshSecs = 1f;
        Vector3 lastPosCheck;

        //QUITAR
        public GameObject goToPoint;
        GameObject aux;

        private void Start()
        {
            movemntToPosition = default(Vector3);
            rb = GetComponent<Rigidbody>();

        }

        private void FixedUpdate()
        {
            if (movemntToPosition == default(Vector3))
            {
                GenerateNewPosition();
            }

            MoveToObjective();

            CheckPosition();
        }

        void GenerateNewPosition()
        {
            var pointOnMap = new Vector2(Random.Range(transform.position.x - movement_Radius, transform.position.x + movement_Radius),
                  Random.Range(transform.position.z - movement_Radius, transform.position.z + movement_Radius));

            movemntToPosition = mapGenerator.GetGlobalPosition(pointOnMap);
            movemntToPosition.y += 0.5f;

            CancelInvoke(nameof(CheckMovedInOneSec));
            InvokeRepeating(nameof(CheckMovedInOneSec), distCheckRefreshSecs, distCheckRefreshSecs);

            aux = Instantiate(goToPoint, movemntToPosition, Quaternion.identity); //QUITAR 
        }

        void MoveToObjective()
        {
            // Calcula la dirección hacia el objetivo
            Vector3 direccion = (movemntToPosition - transform.position).normalized;

            // Mueve el objeto hacia el objetivo
            rb.velocity = new Vector3((direccion * velocity).x, rb.velocity.y, (direccion * velocity).z);
            rb.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);

        }

        void CheckPosition()
        {
            if (Vector3.Distance(transform.position, movemntToPosition) < 2f)
            {
                // Detener el movimiento si el objeto está lo suficientemente cerca del objetivo
                GameObject.Destroy(aux); //QUITAR
                movemntToPosition = default(Vector3);
            }
        }

        void CheckMovedInOneSec()
        {
            if (Vector3.Distance(transform.position, lastPosCheck) < minDistInOneSec)
            {
                GameObject.Destroy(aux); //QUITAR
                movemntToPosition = default(Vector3);
            }
            lastPosCheck = transform.position;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class BotMovement : MonoBehaviour
{
    public float view_Radius;

    public LayerMask targetMask; // layer de los bots
    public LayerMask obstacleMask; //layer de los obstaculos

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [SerializeField]
    MapGenerator mapGenerator;

    [SerializeField]
    float velocity = 5f;
    [SerializeField]
    float gravity = 10f;
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
            var pointOnMap = new Vector2(Random.Range(transform.position.x - view_Radius, transform.position.x + view_Radius),
                Random.Range(transform.position.z - view_Radius, transform.position.z + view_Radius));

            movemntToPosition = mapGenerator.GetGlobalPosition(pointOnMap);
            movemntToPosition.y += 0.5f;

            CancelInvoke(nameof(CheckMovedInOneSec));
            InvokeRepeating(nameof(CheckMovedInOneSec), distCheckRefreshSecs, distCheckRefreshSecs);

            aux = Instantiate(goToPoint, movemntToPosition, Quaternion.identity); //QUITAR 
        }

        // Calcula la dirección hacia el objetivo
        Vector3 direccion = (movemntToPosition - transform.position).normalized;

        // Mueve el objeto hacia el objetivo
        rb.velocity = new Vector3((direccion * velocity).x, rb.velocity.y, (direccion*velocity).z);
        rb.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);

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
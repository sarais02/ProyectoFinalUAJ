using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class BotMovement : MonoBehaviour
{
    public float view_Radius;
    [Range(0, 360)]
    public float view_Angle;

    public LayerMask targetMask; // layer de los bots
    public LayerMask obstacleMask; //layer de los obstaculos

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [SerializeField]
    MapGenerator mapGenerator;

    [SerializeField]
    float velocity = 5f;
    Vector3 movemntToPosition;


    //QUITAR
    public GameObject goToPoint;
    GameObject aux;
    private void Start()
    {
        movemntToPosition = default(Vector3);
       
        movemntToPosition = default(Vector3);
    }

    void Update()
    {
        if (movemntToPosition == default(Vector3))
        {
            var pointOnMap = new Vector2(Random.Range(transform.position.x - view_Radius, transform.position.x + view_Radius),
                Random.Range(transform.position.z - view_Radius, transform.position.z - view_Radius));
            movemntToPosition = mapGenerator.GetGlobalPosition(pointOnMap);
            Instantiate(goToPoint, movemntToPosition, Quaternion.identity); //QUITAR 
        }
        else
        {
            // Calcula la dirección hacia el objetivo
            Vector3 direccion = (movemntToPosition - transform.position).normalized;

            // Calcula la cantidad de movimiento en este frame basado en la velocidad
            float movimiento = velocity * Time.deltaTime;

            // Mueve el objeto hacia el objetivo
            transform.position += direccion * movimiento;

            if (Vector3.Distance(transform.position, movemntToPosition) < 0.2f)
            {
                // Detener el movimiento si el objeto está lo suficientemente cerca del objetivo
                GameObject.Destroy(aux); //QUITAR
                movemntToPosition = default(Vector3);
            }
        }
        
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, view_Radius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < view_Angle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;¿Lo

public class WanderBot : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    [Header("Wander")]
    [SerializeField] float wanderRadius = 50f;
    [SerializeField] float targetYOffset = 0.5f;
    [SerializeField] float movSpeed = 5f;

    [Header("Other Forces")]
    [SerializeField] float gravity = 10f;
    Vector3 gravV3;

    [Header("Debug Help")]
    [SerializeField] GameObject beaconPrefab;

    Vector3 currTargetPos;
    GameObject aux;
    Rigidbody rb;

    // Start is called before the first frame update
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
    }

    void GenerateNewPosition()
    {
        Vector2 pointOnMap = new Vector2(Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius),
              Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius));

        currTargetPos = mapGenerator.GetGlobalPosition(pointOnMap);
        currTargetPos.y += targetYOffset;

        if (beaconPrefab != null)
            aux = Instantiate(beaconPrefab, currTargetPos, Quaternion.identity); //QUITAR

    }
}

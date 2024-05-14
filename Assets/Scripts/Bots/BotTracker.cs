
using System;
using System.Collections.Generic;
using UnityEngine;


public class BotTracker : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float distantePerEvent;
    [SerializeField]
    float timePerEvent;

    float distante;
    Vector3 lastPosition;

    float time;
    Dictionary<string, string> eventParams;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distante = 0f;
        time = 0f;

        lastPosition = rb.position;

        //Parametros para los eventos de movimiento del bot
        eventParams = new Dictionary<string, string>();
        eventParams.Add("posX", "0");
        eventParams.Add("posY", "0");
        eventParams.Add("posZ", "0");
        eventParams.Add("updateByMove", "none");
    }

    void Update()
    {
        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
        distante += distanceThisFrame;
        lastPosition = rb.position;

        time += Time.deltaTime;

        if (distante >= distantePerEvent || time >= timePerEvent)
        {
            updateEventParams(distante >= distantePerEvent);
            //TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.BotPosition);
            Debug.Log("Mandado");
            distante = 0f;
            time = 0f;
        }
    }

    void updateEventParams(bool type)
    {
        eventParams["posX"] = rb.velocity.x.ToString();
        eventParams["posY"] = rb.velocity.y.ToString();
        eventParams["posZ"] = rb.velocity.z.ToString();
        
        if (type)
            eventParams["updateByMove"] = "Distance";
        else
            eventParams["updateByMove"] = "Time";
    }
}

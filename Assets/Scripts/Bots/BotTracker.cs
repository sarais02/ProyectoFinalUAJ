
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrackingBots
{

    public class BotTracker : MonoBehaviour
    {
        Rigidbody rb;

        [SerializeField]
        float distantePerEvent = 5f;
        [SerializeField]
        float timePerEvent = 5f;

        float distante;
        Vector3 lastPosition;

        float time;
        Dictionary<string, string> eventParams;

        [SerializeField] CalculateNavigableAreaController controller;
        public CalculateNavigableAreaController Controller { get { return controller; } set { controller = value; } }


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            distante = 0f;
            time = 0f;

            lastPosition = rb.position;

            eventParams = new Dictionary<string, string>();
            eventParams.Add("posX", "0");
            eventParams.Add("posY", "0");
            eventParams.Add("posZ", "0");
            eventParams.Add("updateByMove", "none");
        }


        void Update()
        {

            if (!controller.TestEnable)
                return;

            float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
            distante += distanceThisFrame;
            lastPosition = rb.position;

            time += Time.deltaTime;

            if (distante >= distantePerEvent || time >= timePerEvent)
            {
                updateEventParams(distante >= distantePerEvent);
                TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.BotPosition, eventParams);
                distante = 0f;
                time = 0f;
            }
        }

        void updateEventParams(bool type)
        {
            eventParams["posX"] = (transform.position.x + 25).ToString();
            eventParams["posY"] = transform.position.y.ToString();
            eventParams["posZ"] = ((transform.position.z - 25)*-1).ToString();

            if (type)
                eventParams["updateByMove"] = "Distance";
            else
                eventParams["updateByMove"] = "Time";
        }
    }
}
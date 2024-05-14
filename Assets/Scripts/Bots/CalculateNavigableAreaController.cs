using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateNavigableAreaController : MonoBehaviour
{

    public uint nBots;

    public GameObject botPrefab;
   
    public Mesh visualBot;  
   
    public Material visualBotMaterial;
    
    public MoveType moveType;

    public float distanceMove, jumpingForce;

    public enum MoveType {  normal, jumping}

    Transform botsParent;

    Dictionary<string, string> eventParams = new Dictionary<string, string>();

    public void StartTest()
    {
        TrackerG5.Tracker.Instance.Init(TrackerG5.Tracker.serializeType.Json, TrackerG5.Tracker.persistenceType.Disc);
        eventParams.Add("nBots", nBots.ToString());
        TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.StartTest, eventParams);
    }
    public void EndTest()
    {
        TrackerG5.Tracker.Instance.AddEvent(TrackerG5.Tracker.eventType.EndTest);
        TrackerG5.Tracker.Instance.End();
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

        for (int i = 0; i < nBots; i++)
        {
            GenerateBot("Bot" + i.ToString());
        }
    }

    GameObject GenerateBot(string name)
    {
        GameObject gO = new GameObject(name, typeof(BotMovement), typeof(Rigidbody), typeof(MeshRenderer), typeof(BotTracker));
        gO.transform.parent = botsParent;
        return gO;
    }
}


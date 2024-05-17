using System.Collections;
using System.Collections.Generic;
using TrackingBots;
using UnityEngine;

public class Reader : MonoBehaviour
{
    public TextAsset json;
    public Config config = new Config();
    // Start is called before the first frame update
    void Start()
    {
        config = JsonUtility.FromJson<Config>(json.text);


    }
}

using UnityEngine;
using UnityEditor;
using System;
using TrackingBots;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameObject generatorObject;
#if UNITY_EDITOR
    private void Start()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            CallMethod("StartTest");
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            CallMethod("EndTest");
        }
    }
#endif
    private void CallMethod(string methodName)
    {
        if (generatorObject == null)
        {
            return;
        }

        var targetScript = generatorObject.GetComponent<CalculateNavigableAreaController>();
        if (targetScript != null)
        {
            var method = targetScript.GetType().GetMethod(methodName);
            if (method != null)
            {
                try
                {
                    method.Invoke(targetScript, null);
                }
                catch
                {
                    Debug.LogError($"Error al invocar el metodo");
                }
            }
    
        }
    }
}
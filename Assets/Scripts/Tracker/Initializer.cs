using UnityEngine;
using UnityEditor;
using System;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameObject generatorObject;

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
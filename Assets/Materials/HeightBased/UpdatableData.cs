using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject
{

	public event System.Action OnValuesUpdated;
	public bool autoUpdate;


	protected virtual void OnValidate()
	{
		if (autoUpdate)
		{
            // Ejecuta el metodo NotifyOfUpdatedValues despues de que compilen los shaders,
            // para aplicar los valores
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
#endif
        }
    }

	public void NotifyOfUpdatedValues()
	{
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
#endif
        if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

}
using UnityEditor;
using UnityEngine;

namespace TrackingBots
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CalculateNavigableAreaController))]
    public class CalculateNavigableEditor : Editor
    {
        CalculateNavigableAreaController controller;
        public override void OnInspectorGUI()
        {
            controller = (CalculateNavigableAreaController)target;
            if (controller == null)
                return;

            EditorGUILayout.Space();
            GUILayout.Label("Calculate Navigable Area", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
            GUILayout.Label("Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
            EditorGUILayout.Space();
            ////
            EditorGUILayout.Space();
            GUILayout.Label("Elementos generales", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
            ////

            EditorGUI.BeginChangeCheck();//Para poder ver si se han producido cambios en el editor


            EditorGUILayout.PropertyField(serializedObject.FindProperty("nBots"), new GUIContent("Numero de Bots"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleTimeInTest"), new GUIContent("Velocidad del juego durante el test"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTimeTest"), new GUIContent("Duraci�n del test (en segundos)"));


            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPoint"), new GUIContent("Posici�n central de la generaci�n de los bots"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDispersionBots"), new GUIContent("Dispersi�n de los bots desde el origen"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainMask"), new GUIContent("M�scara del terreno"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHeightOfTheMap"), new GUIContent("M�xima altura del mapa"));

            EditorGUILayout.Space();
            GUILayout.Label("Movimiento de los bots", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });


            EditorGUILayout.PropertyField(serializedObject.FindProperty("wanderRadius"), new GUIContent("Amplitud de movimiento"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wanderRandomRelative"), new GUIContent("Desviaci�n media"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colliderMat"), new GUIContent("Material f�sico de los bots"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Visualizaci�n", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("visualBot"), new GUIContent("Est�tica de los bots"));

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("json"), new GUIContent("JSON"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Generar bots"))
            {
                controller.GenerateBots();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            GUILayout.Label("Se deben de activar y desactivar los tests fuera de ejecuci�n",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 8 });
            if (GUILayout.Button("Iniciar test"))
            {
                if (Application.isEditor && !Application.isPlaying)
                    controller.StartTest();
            }

            if (GUILayout.Button("Finalizar test"))
            {
                if (Application.isEditor && !Application.isPlaying)
                    controller.EndTest();
            }

            EditorGUILayout.Space();

            if (controller.TestEnable)
            {
                GUILayout.Label("TESTS EN MARCHA!", new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14 });
            }
            else
            {

                GUILayout.Label("LOS BOT Y TESTS EST�N DESACTIVADOS", new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14 });

            }

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
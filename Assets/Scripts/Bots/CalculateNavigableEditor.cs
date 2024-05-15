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



            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPoint"), new GUIContent("Posición central de la generación de los bots"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDispersionBots"), new GUIContent("Dispersión de los bots desde el origen"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainMask"), new GUIContent("Máscara del terreno"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHeightOfTheMap"), new GUIContent("Máxima altura del mapa"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mapAssociated"), new GUIContent("Mapa asociado - quitar"));

            EditorGUILayout.Space();
            GUILayout.Label("Movimiento de los bots", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveType"), new GUIContent("Tipo de movimiento de los bots"));

            if (controller.moveType == CalculateNavigableAreaController.MoveType.normal)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("wanderRadius"), new GUIContent("Amplitud de movimiento"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("wanderRandomRelative"), new GUIContent("Desviación media"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colliderMat"), new GUIContent("Material físico de los bots"));
            }
            else if (controller.moveType == CalculateNavigableAreaController.MoveType.jumping)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpingForce"), new GUIContent("Fuerza de salto"));
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Visualización", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("visualBot"), new GUIContent("Estética de los bots"));

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

            if (GUILayout.Button("Iniciar test"))
            {
                controller.StartTest();
            }
           
            if (GUILayout.Button("Finalizar test"))
            {
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

                GUILayout.Label("LOS BOT Y TESTS ESTÁN DESACTIVADOS", new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14 });

            }


            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
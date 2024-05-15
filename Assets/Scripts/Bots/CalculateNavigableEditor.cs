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
            GUILayout.Label("Elementos básicos", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
            ////

            EditorGUI.BeginChangeCheck();//Para poder ver si se han producido cambios en el editor


            EditorGUILayout.PropertyField(serializedObject.FindProperty("nBots"), new GUIContent("Numero de Bots"));


            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveType"), new GUIContent("Tipo de desplazamiento de los bots"));

            if (controller.moveType == CalculateNavigableAreaController.MoveType.normal)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceMove"), new GUIContent("Distancia por movimiento"));
            }
            else if (controller.moveType == CalculateNavigableAreaController.MoveType.jumping)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpingForce"), new GUIContent("Fuerza de salto"));
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label("Visualización", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("visualBot"), new GUIContent("Componente visual"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("visualBotMaterial"), new GUIContent("Material personalizable"));

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

            serializedObject.ApplyModifiedProperties();

        }
        private void OnDisable()
        {
            if (controller != null && controller.testEnable)
                controller.EndTest();
        }
    }
#endif
}
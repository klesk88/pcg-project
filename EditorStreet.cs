using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(StreetCreation))]

public class EditorStreet : Editor {

   


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls();

        StreetCreation street_creation = (StreetCreation)target as StreetCreation;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Reads Road");
        street_creation.readRoads = EditorGUILayout.Toggle(street_creation.readRoads);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Generate All Roads");
        street_creation.generateAllRoads = EditorGUILayout.Toggle(street_creation.generateAllRoads);

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        Rect startButton = EditorGUILayout.BeginHorizontal();
        startButton.x = startButton.width / 2 - 100;
        startButton.width = 200;
        startButton.height = 18;



        if (GUI.Button(startButton, "Create Streets"))
        {
            //street_creation.createBuilding();

            GUIUtility.ExitGUI();
        }



        if (GUI.changed)
        {

            EditorUtility.SetDirty(street_creation);
        }
    }

}

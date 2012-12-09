using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildingCreation))]

public class EditorBuilding : Editor
{

    private int number_of_buildings;
    private bool enter = false;
    
    void setNumberOfBuildings()
    {
     
            BuildingCreation building_creation = (BuildingCreation)target as BuildingCreation;
            //read the file so i know how many buildings are 
            building_creation.readBuildings();
            number_of_buildings = building_creation.getNumberOfBuildings();
            Debug.Log("inside " + number_of_buildings);
        
    }

   
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls();

        BuildingCreation building_creation = (BuildingCreation)target as BuildingCreation;
        
       
        
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Generate All Buildings");
        building_creation.generateAllBuildings = EditorGUILayout.Toggle(building_creation.generateAllBuildings);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

     
           
            EditorGUILayout.Separator();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            building_creation.buildingsToGenerate = (int)EditorGUILayout.IntSlider("Buildings to Generate", building_creation.buildingsToGenerate, 1,number_of_buildings );
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Separator();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            building_creation.scalingFactor = (float)EditorGUILayout.Slider("Scaling Factor", building_creation.scalingFactor, 1, 100);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            Rect startButton = EditorGUILayout.BeginHorizontal();
            startButton.x = startButton.width / 2 - 100;
            startButton.width = 200;
            startButton.height = 18;

            if (!enter)
            {
                setNumberOfBuildings();
                enter = true;
            }
            if (GUI.Button(startButton, "Create Buildings"))
            {
                building_creation.createBuilding();

                GUIUtility.ExitGUI();
            }

 

        if (GUI.changed)
        {


         
            EditorUtility.SetDirty(building_creation);

     
        }
    }

}
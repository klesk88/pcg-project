using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildingCreation))]

public class EditorBuilding : Editor
{

    private int number_of_buildings;
    private bool enter = false;
    private bool destroy = false;
    
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
        EditorGUILayout.PrefixLabel("L-System Axiom");
        building_creation.axiom = EditorGUILayout.TextField(building_creation.axiom);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("L-System Production Rule");
        building_creation.productionRule = EditorGUILayout.TextField(building_creation.productionRule);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("L-System Expansion");
        building_creation.expansions = EditorGUILayout.IntSlider(building_creation.expansions,1,5);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("L-System Height Max");
        building_creation.heightMax = EditorGUILayout.Slider(building_creation.heightMax,3,10);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("L-System Height Min");
        building_creation.heightMin = EditorGUILayout.Slider(building_creation.heightMin, 0.5f, 5);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

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
                enter = false;
                building_creation.createBuilding();
                DestroyImmediate(building_creation);
                GUIUtility.ExitGUI();
            }

 

        if (GUI.changed)
        {
         
            EditorUtility.SetDirty(building_creation);
            if (destroy) {
               
               
            }
                
     
        }
    }

}
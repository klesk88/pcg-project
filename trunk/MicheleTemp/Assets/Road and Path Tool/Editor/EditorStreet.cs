using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(StreetCreation))]

public class EditorStreet : Editor {
    Ray ray = new Ray();
    RaycastHit hit;
    Node startNode, endNode;
    bool endSelect = false;
    bool waitForAStar = false;
    bool rStarted = false;

    private int number_of_streets;
    private bool enter = false;


    void setNumberOfStreets() {

        StreetCreation street_creation = (StreetCreation)target as StreetCreation;
        //read the file so i know how many buildings are 
      
        
        number_of_streets = street_creation.getNumberOfStreets();
       

    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls();

        StreetCreation street_creation = (StreetCreation)target as StreetCreation;


        if (street_creation.emptyDictionary()) {

            setNumberOfStreets();
            street_creation.getData();
        }

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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
   
        street_creation.roadsToGenerate = EditorGUILayout.IntSlider("Generate Amount Of Roads",street_creation.roadsToGenerate, 1, number_of_streets);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();




        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Car Prefab");
        street_creation.car = (GameObject)EditorGUILayout.ObjectField(street_creation.car, typeof(GameObject));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Chacpoint");
        street_creation.checkpoint = (GameObject)EditorGUILayout.ObjectField(street_creation.checkpoint, typeof(GameObject));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Mesh Car");
        street_creation.carHelper = (GameObject)EditorGUILayout.ObjectField(street_creation.carHelper, typeof(GameObject));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        Rect startButton = EditorGUILayout.BeginHorizontal();
        startButton.x = startButton.width / 2 - 100;
        startButton.width = 200;
        startButton.height = 18;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
     

        if (GUI.Button(startButton, "Create Streets"))
        {
            //street_creation.createStreets() ;

            //GUIUtility.ExitGUI();
        }



        if (GUI.changed)
        {

            EditorUtility.SetDirty(street_creation);
        }
    }

    public void OnSceneGUI() {

        StreetCreation street_creation = (StreetCreation)target as StreetCreation;

        Event currentEvent = Event.current;

        if (street_creation.emptyDictionary()) {
            street_creation.getData();
        }
      

     //   if (currentEvent.type == EventType.keyUp && currentEvent.character == 'r')
     //       Debug.Log("EventType: " + currentEvent.type);


        if (currentEvent.type == EventType.KeyUp /* && currentEvent.character == 'r'*//*Input.GetKey(KeyCode.R)*/) {
            //street_creation.updatePathfinder();
            //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
            if (Physics.Raycast(ray, out hit) && currentEvent.keyCode == KeyCode.R) {
                Debug.Log("After Physics Raycast");
                Debug.Log("hit.point:" + hit.point);
                if (!endSelect) {
                    street_creation.path_finder.startNode = street_creation.path_finder.nearestNode(hit.point);
                       // startNode.getGameObject().renderer.material.color = Color.green;
                    Debug.Log("START NODE COORDINATES: " + street_creation.path_finder.startNode.getPosition());
                    endSelect = true;
                }
                else {
                    street_creation.path_finder.endNode = street_creation.path_finder.nearestNode(hit.point);
                  //   endNode.getGameObject().renderer.material.color = Color.green;
                    Debug.Log("GOAL NODE COORDINATES: " + street_creation.path_finder.endNode.getPosition());
                    waitForAStar = true;
                    ArrayList bestPath = street_creation.path_finder.FindPath();
                    endSelect = false;
                }
            }
       
          
        }



        if (GUI.changed) {

            EditorUtility.SetDirty(street_creation);
        }
    }

}

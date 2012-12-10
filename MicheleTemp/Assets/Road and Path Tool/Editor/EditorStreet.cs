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
    bool rStarted = false, checkpointPlacement = false, carPlacement = false;
    float timeStamp = 0;

    private GameObject checkpoint = null, car = null;
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
        EditorGUILayout.PrefixLabel("Checkpoint Prefab");
        street_creation.checkpoint = (GameObject)EditorGUILayout.ObjectField(street_creation.checkpoint, typeof(GameObject));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Car Mesh");
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

        if (currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.R) {
            ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
            if (Physics.Raycast(ray, out hit)) {
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
        else if(currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.D) {
            ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
            if(Physics.Raycast(ray, out hit)) {                
                if(!checkpointPlacement) {
                    checkpointPlacement = true;
                    checkpoint = (GameObject)Instantiate(street_creation.path_finder.checkpoint, hit.point, Quaternion.identity);
                }
                else{
                    checkpointPlacement = false;
                    Debug.Log("Checkpoint PLACED on track!");
                }
            }
        }
        else if(currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.C) {
            ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
            if(Physics.Raycast(ray, out hit)) {                
                if(!carPlacement) {
                    carPlacement = true;
                    car = (GameObject)Instantiate(street_creation.path_finder.carHelper, hit.point, Quaternion.identity);
                }
                else{
                    carPlacement = false;
                    Instantiate(street_creation.path_finder.car, car.transform.position + new Vector3(0,5,0), car.transform.rotation).name = "car";
                    DestroyImmediate(car);
                    EditorApplication.isPlaying = true;
                    Debug.Log("Car PLACED on track!");
                }
            }
        }
        else if(currentEvent.type == EventType.MouseUp && currentEvent.button == 1) {
            if(checkpointPlacement && checkpoint != null) {
                Debug.Log("Should destroy checkpoint!");
                DestroyImmediate(checkpoint);
                checkpointPlacement = false;
            }
            else if(carPlacement && car != null) {
                Debug.Log("Should destroy car!");
                DestroyImmediate(car);
                carPlacement = false;
            }
        }
        else if(currentEvent.type == EventType.MouseMove) {
            if(checkpointPlacement) {
                if(!currentEvent.shift) {
                    ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
                    if(Physics.Raycast(ray, out hit)) {
                        checkpoint.transform.position = hit.point;
                    }
                }
                else {
                    checkpoint.transform.RotateAround(Vector3.up, currentEvent.delta.x/20f);
                }
            }
            else if(carPlacement) {
                if(!currentEvent.shift) {
                    ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
                    if(Physics.Raycast(ray, out hit)) {
                        car.transform.position = hit.point;
                    }
                }
                else {
                    car.transform.RotateAround(Vector3.up, currentEvent.delta.x / 20f);
                }
            }
        }

        if (GUI.changed) {

            EditorUtility.SetDirty(street_creation);
        }
    }

}

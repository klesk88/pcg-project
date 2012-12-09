using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AttachedPathScript))]

public class SmoothEditor : Editor{
    bool endSelect = false;
    Ray ray = new Ray();
    RaycastHit hit;

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        AttachedPathScript aps = (AttachedPathScript)target as AttachedPathScript;




        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.PrefixLabel("Reads Road");
        aps.number_of_iterations = EditorGUILayout.IntSlider(aps.number_of_iterations,1,400);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

      


        if (GUI.changed) {

            EditorUtility.SetDirty(aps);
        }
    }



    public void OnSceneGUI() {

        Event currentEvent = Event.current;

        AttachedPathScript aps = (AttachedPathScript)target as AttachedPathScript;
        ray = Camera.current.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25)));
        if (currentEvent.type == EventType.KeyUp) {
            if (Physics.Raycast(ray, out hit) && currentEvent.keyCode == KeyCode.U) {
                Debug.Log("inside");
                //AttachedPathScript aps = Selection.activeGameObject.GetComponent<AttachedPathScript>();

                if (!endSelect) {
                    aps.click_coordinates = new List<PathNodeObjects>();
                    int index = aps.nearestNode(hit.point);
                    Debug.Log("startIndex: " + index);
                    aps.click_coordinates.Add(aps.nodeObjects[index - 1]);
                    aps.click_coordinates.Add(aps.nodeObjects[index]);

                    //    startNode.getGameObject().renderer.material.color = Color.green;
                    Debug.Log("START NODE COORDINATES: " + aps.nodeObjects[index - 1].position + " "  + aps.nodeObjects[index].position);
                    endSelect = true;
                }
                else {
                    Debug.Log("asdsad ");
                    int index = aps.nearestNode(hit.point);
                    aps.click_coordinates.Add(aps.nodeObjects[index]);
                    aps.click_coordinates.Add(aps.nodeObjects[index + 1]);
                    Debug.Log("endIndex: " + index);
                    //    endNode.getGameObject().renderer.material.color = Color.green;
                    Debug.Log("GOAL NODE COORDINATES: " + aps.nodeObjects[index].position + " " + aps.nodeObjects[index+1].position);
                    aps.smoothPath();

                    endSelect = false;
                }
            }
        }
    }
}

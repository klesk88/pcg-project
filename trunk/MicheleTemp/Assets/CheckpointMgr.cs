using UnityEngine;
using System.Collections;

public class CheckpointMgr : MonoBehaviour {

    public static Vector3 latestCheckpoint = Vector3.zero;
    public static Quaternion orientation = Quaternion.identity;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.name == "Collider_Bottom") {
            Debug.Log("Checkpoint passed! Press T to start from this location");
            latestCheckpoint = transform.Find("spawn").position;
            orientation = transform.rotation;
            orientation *= Quaternion.Euler(new Vector3(0, -90, 0));
        }
    }
}

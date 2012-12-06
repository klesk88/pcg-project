using UnityEngine;
using System.Collections;

public class CheckpointMgr : MonoBehaviour {

    public static Vector3 latestCheckpoint = Vector3.zero;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.name == "Collider_Bottom") {
            Debug.Log("Checkpoint passed! Press T to start from this location");
            latestCheckpoint = transform.position + new Vector3(10, 0, 0);
        }
    }
}

using UnityEngine;
using System.Collections;

public class BuildingDestroyer : MonoBehaviour {

    void Start() {
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.gameObject.name.Contains("Building")) {
            Debug.Log("I'm getting destroooyed");
            Destroy(this.gameObject);
        }
    }
}

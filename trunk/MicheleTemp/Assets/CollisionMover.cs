using UnityEngine;
using System.Collections;

public class CollisionMover : MonoBehaviour {

    /**
     * This was for the buildings
    void OnCollisionStay(Collision collisionInfo) {
        Debug.Log(collisionInfo.gameObject.name);
            transform.Translate(new Vector3(15, 0, 0));
    }
     */

    void OnCollisionStay(Collision collisionInfo) {
        if(collisionInfo.gameObject.name.Contains("Building"))
            collisionInfo.gameObject.transform.Translate(new Vector3(10, 0, 0));
    }

}

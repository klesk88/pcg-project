using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class BuildingDestroyer : MonoBehaviour {

    public bool colliding() {
        Collider[] nearbyColliders = Physics.OverlapSphere(gameObject.transform.position, gameObject.GetComponent<BoxCollider>().bounds.extents.magnitude);
        if (nearbyColliders != null && nearbyColliders.Length > 0) {
            foreach (Collider collider in nearbyColliders) {
                if (collider.gameObject.name.Contains("Path"))
                    return true;
            }
            
        }
        return false;
    }
}

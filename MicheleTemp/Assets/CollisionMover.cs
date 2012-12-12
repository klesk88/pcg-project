using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class CollisionMover : MonoBehaviour {

    public void check() {
        Bounds bounds = gameObject.GetComponent<MeshCollider>().bounds;
        Collider[] nearbyColliders = Physics.OverlapSphere(bounds.center, bounds.extents.magnitude);
        if(nearbyColliders != null && nearbyColliders.Length > 0){
            foreach (Collider collider in nearbyColliders) {
                if (gameObject.GetComponent<MeshCollider>().bounds.Intersects(collider.bounds) && collider.gameObject.name.Contains("Building")) {
                    if (collider.gameObject.GetComponent<BuildingDestroyer>() == null)
                        collider.gameObject.AddComponent<BuildingDestroyer>();
                    if (collider.gameObject.GetComponent<BuildingDestroyer>().colliding())
                        DestroyImmediate(collider.gameObject);
                }
            }
        }
    }

}

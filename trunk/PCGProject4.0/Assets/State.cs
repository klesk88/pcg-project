using UnityEngine;
using System.Collections;

public class State/* : MonoBehaviour*/ {
    private GameObject gameObject;
    private Vector3[] face;
    private Vector3 pivotOffset;

    public State(GameObject _gameObject, Vector3[] _face) {
        gameObject = _gameObject;
        face = _face;
        pivotOffset = Vector3.zero;
    }

    public State() { }

    public GameObject getObject() {
        return gameObject;
    }

    public Vector3[] getFace() {
        return face;
    }

    public Vector3[] getTop(){
        if(gameObject.GetComponent<MeshFilter>() == null)
            return face;
        Vector3[] verts = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] topVerts = new Vector3[verts.Length / 2];
        for(int i = 0; i < topVerts.Length; i++) {
            topVerts[i].x = verts[i + topVerts.Length].x; /* gameObject.transform.localScale.x + gameObject.transform.position.x;*//* + offset()*//*gameObject.transform.position*/
            topVerts[i].y = verts[i + topVerts.Length].y; /** gameObject.transform.localScale.y + gameObject.transform.position.y;*/
            topVerts[i].z = verts[i + topVerts.Length].z; /** gameObject.transform.localScale.z + gameObject.transform.position.z;*/
        }
        return topVerts;
    }

    public Vector3 offset() {
        return new Vector3(gameObject.transform.position.x * gameObject.transform.localScale.x, gameObject.transform.position.y * gameObject.transform.localScale.y, gameObject.transform.position.z * gameObject.transform.localScale.z);
    }

    public Vector3 rootOffset() {
        Transform current = gameObject.transform;
        Vector3 rootOffset = Vector3.one;
        while(current.gameObject.name != "testBuilding") {
            rootOffset.x *= (current.position.x * current.localScale.x);
            rootOffset.y *= (current.position.y * current.localScale.y);
            rootOffset.z *= (current.position.z * current.localScale.z);
            current = current.parent;
        }
        return rootOffset;
    }

    public void setObject(GameObject _gameObject) {
        gameObject = _gameObject;
    }

    public void setFace(Vector3[] _face) {
        face = _face;
    }

    public void setPivotOffset(Vector3 _pivotOffset){
        pivotOffset = _pivotOffset;
    }

    public Vector3 getPivotOffset() {
        return pivotOffset;
    }
    

    public State clone() {
        return new State(gameObject, face);
    }
}

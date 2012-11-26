using UnityEngine;
using System.Collections;

public class StateOld/* : MonoBehaviour*/ {
    private GameObject gameObject;
    private Vector3[] face;

    public StateOld(GameObject _gameObject, Vector3[] _face) {
        gameObject = _gameObject;
        face = _face;
    }

    public StateOld() { }

    public GameObject getObject() {
        return gameObject;
    }

    public Vector3[] getFace() {
        return face;
    }

    public void setObject(GameObject _gameObject) {
        gameObject = _gameObject;
    }

    public void setFace(Vector3[] _face) {
        face = _face;
    }

    public StateOld clone() {
        return new StateOld(gameObject, face);
    }
}
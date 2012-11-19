using UnityEngine;
using System.Collections;

public class State/* : MonoBehaviour*/ {
    private GameObject gameObject;
    private Vector3[] face;

    public State(GameObject _gameObject, Vector3[] _face) {
        gameObject = _gameObject;
        face = _face;
    }

    public State() { }

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

    public State clone() {
        return new State(gameObject, face);
    }
}

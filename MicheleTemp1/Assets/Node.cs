using UnityEngine;
using System.Collections;

public class Node {
    GameObject gameObject;
    ArrayList neighbours;
    Vector3 position;

    public Node(Vector3 _position, Vector3[] _neighbours, GameObject _gameObject = null) {
        position = _position;
        /*if (_gameObject != null) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(cube.GetComponent<BoxCollider>());
            cube.transform.position = position;
            cube.transform.localScale = new Vector3(1.5f, 0.2f, 0.6f);
            cube.transform.parent = _gameObject.transform;
            float zxRelation = (_neighbours.Length == 2 ? (_neighbours[0].z - _neighbours[1].z) / ((_neighbours[0].x - _neighbours[1].x != 0 ? _neighbours[0].x - _neighbours[1].x : 1)) : (_position.z - _neighbours[0].z) / ((_position.x - _neighbours[0].x != 0 ? _position.x - _neighbours[0].x : 1)));
            cube.transform.Rotate(new Vector3(0, -Mathf.Rad2Deg * Mathf.Atan(zxRelation), 0));
            gameObject = cube;
        }
        else*/
            gameObject = _gameObject;
        
        neighbours = new ArrayList(); 
        foreach (Vector3 pos in _neighbours)
            neighbours.Add(pos);
    }

    public void addNeighbours(Vector3[] newNeighbours) {
        foreach (Vector3 neighbour in newNeighbours)
            neighbours.Add(neighbour);
    }

    public GameObject getGameObject() { return gameObject; }
    public Vector3 getPosition() { return position; }

    public void getNeighbours(ArrayList _neighbours) {
        _neighbours.Clear();
        for (int i = 0; i < neighbours.Count; i++)
            _neighbours.Add(PathFinder.nodeMap[(Vector3)neighbours[i]]);
    }
}

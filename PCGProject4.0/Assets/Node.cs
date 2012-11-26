using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    GameObject gameObject;
    ArrayList neighbours;
    Node parent;
    Vector3 position;
    int totalCost;

    public Node(Vector3[] _neighbours, GameObject _gameObject = null) {
        gameObject = _gameObject;
        foreach (Vector3 pos in _neighbours)
            neighbours.Add(pos);
    }

    public void addNeighbours(Vector3[] newNeighbours) {
        foreach (Vector3 neighbour in newNeighbours)
            neighbours.Add(neighbour);
    }

    public GameObject getGameObject() { return gameObject; }
    public Vector3 getPosition() { return position; }
    public Node getParent() { return parent; }
    public int getTotalCost() { return totalCost; }
//    bool isGoal(Node goalNode){

    public void setGameObject(GameObject _gameObject) {
        gameObject = _gameObject;
    }

    public void setPosition(Vector3 _position) {
        position = _position;
        gameObject.transform.position = position;
    }

    public void setParent(Node _parent) {
        parent = _parent;
    }

    public void setTotalCost(int _totalCost) {
        totalCost = _totalCost;
    }

    public void getSuccessors(ArrayList _successors) {
        _successors.Clear();
        for (int i = 0; i < neighbours.Count; i++)
            _successors.Add(PathFinder.nodeMap[(Vector3)neighbours[i]]);
    }




    // Use this for initialization
    void Start() {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

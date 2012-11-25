using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is still work in progress

public class PathFinder : MonoBehaviour {
    ArrayList nodes = new ArrayList();
    Ray ray = new Ray();
    RaycastHit hit;
    Node startNode, endNode;
    bool endSelect = false;
    bool waitForAStar = false;
    public static Dictionary<Vector3, Node> nodeMap = new Dictionary<Vector3, Node>();


	// Use this for initialization
	void Start () {
	
	}

    void addNode(Vector3 pos) {
        GameObject nodeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ///Node node = new Node(nodeObject);
        //nodes.Add(node);
       // node.setPosition(pos);
    }

    void selectNode(bool first) {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!waitForAStar) {
            if (Input.GetMouseButtonUp(0)) {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            if (Physics.Raycast(ray, out hit)) {
                if (!endSelect) {
                   // startNode = new Node(hit.collider.gameObject);
                   // hit.po
                   // endSelect = true;
                }
                else {
                    //endNode = new Node(hit.collider.gameObject);
                    FindPath();
                    waitForAStar = true;
                }
            }
        }
	}

    ArrayList FindPath() {
        ArrayList path = new ArrayList();

        ArrayList open = new ArrayList();
        ArrayList closed = new ArrayList();

        ArrayList successors = new ArrayList();

        open.Add(startNode);

        while (open.Count > 0) {
            Node current = (Node)open[open.Count-1];
            open.RemoveAt(open.Count - 1);
            if(current == endNode){
                while (current != null) {
                    path.Add(current);
                    current = current.getParent();
                }
                break;
            }
            current.getSuccessors(successors);
            foreach (Node successor in successors) {
                Node isOpen = null;
                if (open.Contains(successor))
                    isOpen = (Node)open[open.IndexOf(successor)];
                if (isOpen != null && successor.getTotalCost() > isOpen.getTotalCost())
                    continue;
                Node isClosed = null;
                if (closed.Contains(successor))
                    isClosed = (Node)closed[closed.IndexOf(successor)];
                if (isClosed != null && successor.getTotalCost() > isClosed.getTotalCost())
                    continue;

                open.Remove(isOpen);
                closed.Remove(isClosed);
                open.Add(successor);
            }
            closed.Add(current);
        }

        waitForAStar = false;
        return path;
    }
}

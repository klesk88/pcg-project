using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {
    Ray ray = new Ray();
    RaycastHit hit;
    Node startNode, endNode;
    bool endSelect = false;
    bool waitForAStar = false;

    Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
    Dictionary<Node, float> gScore = new Dictionary<Node, float>();
    Dictionary<Node, float> fScore = new Dictionary<Node, float>();

    ArrayList path = new ArrayList();
    ArrayList open = new ArrayList();
    ArrayList closed = new ArrayList();
    ArrayList neighbours = new ArrayList();
    ArrayList pathPoints = new ArrayList();

    public static Dictionary<Vector3, Node> nodeMap = new Dictionary<Vector3, Node>();
 	
	// Update is called once per frame
	void Update () {
        if (!waitForAStar) {
            if (Input.GetMouseButtonUp(0)) {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    if (!endSelect) {
                        startNode = nearestNode(hit.point);
                        startNode.getGameObject().renderer.material.color = Color.green;
                        Debug.Log("START NODE COORDINATES: " + startNode.getPosition());
                        endSelect = true;
                    }
                    else {
                        endNode = nearestNode(hit.point);
                        endNode.getGameObject().renderer.material.color = Color.green;
                        Debug.Log("GOAL NODE COORDINATES: " + endNode.getPosition());
                        waitForAStar = true;
                        ArrayList bestPath = FindPath();
                        endSelect = false;
                    }
                }
            }
        }
	}

    ArrayList FindPath() {
        cameFrom.Clear();
        gScore.Clear();
        fScore.Clear();
        path.Clear();
        open.Clear();
        closed.Clear();
        neighbours.Clear();
        clearPathPoints();

        float tentativeGScore = 0;
        gScore[startNode] = 0;
        fScore[startNode] = gScore[startNode] + Vector3.Distance(startNode.getPosition(), endNode.getPosition());
        open.Add(startNode);

        int i = 0;
        while (open.Count > 0 && i < 10000) {
            Node current = lowestFScore();
            if(current == endNode){
                while (current != null) {
                    path.Add(current);
                    current = cameFrom.ContainsKey(current) ? cameFrom[current] : null;
                }
                break;
            }
            open.RemoveAt(open.IndexOf(current));
            closed.Add(current);
            current.getNeighbours(neighbours);
            foreach (Node neighbour in neighbours) {
                if (closed.Contains(neighbour))
                    continue;

                tentativeGScore = gScore[current] + Vector3.Distance(current.getPosition(), neighbour.getPosition());

                if (!open.Contains(neighbour) || tentativeGScore <= gScore[neighbour]) {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = gScore[neighbour] + Vector3.Distance(neighbour.getPosition(), endNode.getPosition());
                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
            i++;
        }
        if (path.Count > 0)
            Debug.Log("Path succesfully found!" + "Path Distance: " + tentativeGScore + " m (estimate)");
        else {
            Debug.Log("It was not possible to find a path.");
            startNode.getGameObject().renderer.material.color = Color.grey;
            endNode.getGameObject().renderer.material.color = Color.grey;
        }
        //foreach (Node node in path)
        //    node.getGameObject().renderer.material.color = Color.red;
        visualizePath();
        waitForAStar = false;
        return path;
    }

    Node lowestFScore() {
        Node lowestNode = new Node(Vector3.zero, new Vector3[1]);
        float lowestfScore = Mathf.Infinity;
        foreach (Node node in open) {
            if (fScore[node] < lowestfScore) {
                lowestfScore = fScore[node];
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    public Node nearestNode(Vector3 pos) {
        float closestDist = Mathf.Infinity;
        Vector3 closestNode = Vector3.zero;
        foreach (Vector3 nodePos in nodeMap.Keys) {
            float dist = Vector3.Distance(pos, nodePos);
            if (dist < closestDist) {
                closestDist = dist;
                closestNode = nodePos;
            }
        }
        return (Node)nodeMap[closestNode];
    }

    public void visualizePath() {
        foreach(Node node in path) {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.renderer.material.color = Color.blue;
            point.transform.position = node.getPosition();
            point.transform.localScale = new Vector3(5, 5, 5);
            Destroy(point.GetComponent<SphereCollider>());
            pathPoints.Add(point);
        }
    }

    public void clearPathPoints() {
        foreach(GameObject point in pathPoints)
            Destroy(point);
        pathPoints.Clear();
    }
}
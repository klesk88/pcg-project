using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

public class Data : MonoBehaviour {
    public int buildingsToGenerate = 100;
    public bool generateAllBuildings = false;
    public float scalingFactor = 20;
    public bool readRoads = false;
    public int roadsToGenerate = 100;
    public bool generateAllRoads = false;

	// Use this for initialization
	void Start () {
        XMLParser parser = new XMLParser();
        //parser.xmlParser();
        XMLParser.FileToCreate[] type_of_file = new XMLParser.FileToCreate[2];
        type_of_file[0] = XMLParser.FileToCreate.Buildings;
        type_of_file[1] = XMLParser.FileToCreate.Streets;
        //parser.deleteAllFiles();
        //parser.createFile(type_of_file);
        List<List<double[]>> data = new List<List<double[]>>(); 
        Debug.Log("before: " + data.Count);
        data = parser.read("Building.txt");
        Debug.Log("after: " + data.Count);
        //    Debug.Log(f);
        //LSystem lsystem = new LSystem();
        //foreach(float f in data[3][3])
        LSystem lsystem = this.gameObject.GetComponent<LSystem>();
        lsystem.init();

        for (int i = 0; i < (generateAllBuildings ? data.Count : buildingsToGenerate); i++) {
            if(data[i].Count <= 2)
                continue;
            Vector3[] vertices = new Vector3[data[i].Count-1];
            GameObject building = new GameObject();
            building.name = "Building_" + i;
            building.transform.parent = this.gameObject.transform;
            int j = 0;
            int height = 0;
            float minX = Mathf.Infinity, maxX = -Mathf.Infinity, minZ = Mathf.Infinity, maxZ = -Mathf.Infinity;
            foreach (double[] dArray in data[i]) {
                if (j < data[i].Count - 1) { // Ignore last node, since it's equal to the first
                    vertices[j].x = (float)dArray[0] * scalingFactor;
                    vertices[j].z = (float)dArray[1] * scalingFactor;
                    if (vertices[j].x < minX) minX = vertices[j].x;
                    if (vertices[j].x > maxX) maxX = vertices[j].x;
                    if (vertices[j].z < minZ) minZ = vertices[j].z;
                    if (vertices[j].z > maxZ) maxZ = vertices[j].z;
                    //Debug.Log("x: " + vertices[j].x + "   z; " + vertices[j].z);
                    if (dArray[2] != 0)
                        height = (int)dArray[2];
                    j++;
                }   
            }
            Vector3 offset = new Vector3(minX + ((maxX - minX) / 2), 0, minZ + ((maxZ - minZ) / 2));
            for(j = 0; j < vertices.Length; j++)
                vertices[j] -= offset;
            float randHeight = lsystem.randHeight();
            building.AddComponent("BoxCollider");
            building.GetComponent<BoxCollider>().center = new Vector3(0, randHeight / 2, 0);
            building.GetComponent<BoxCollider>().size = new Vector3(maxX - minX, randHeight, maxZ - minZ);

            if(height == 0)
                lsystem.visualize(building/*this.gameObject*/, vertices, offset, randHeight);
         //   else
         //      lsystem.visualize(building, vertices, height);
            if (i % 100 == 0 && i > 0)
                Debug.Log("Buildings created: " + i);
        }

        if (readRoads) {
            List<List<double[]>> roadData = new List<List<double[]>>();
            roadData = parser.read("Highway.txt");
            for (int i = 0; i < (generateAllRoads ? roadData.Count : roadsToGenerate); i++) {
                if (roadData[i].Count <= 2)
                    continue;
                for (int j = 0; j < roadData[i].Count; j++) {
                    Vector3 pos = new Vector3((float)roadData[i][j][0] * scalingFactor, 0, (float)roadData[i][j][1] * scalingFactor);
                    Vector3[] neighbours;
                    if (j == 0) {
                        neighbours = new Vector3[1];
                        neighbours[0] = new Vector3((float)roadData[i][j + 1][0] * scalingFactor, 0, (float)roadData[i][j + 1][1] * scalingFactor);
                    }
                    else if (j == roadData[i].Count - 1) {
                        neighbours = new Vector3[1];
                        neighbours[0] = new Vector3((float)roadData[i][j - 1][0] * scalingFactor, 0, (float)roadData[i][j - 1][1] * scalingFactor);
                    }
                    else {
                        neighbours = new Vector3[2];
                        neighbours[0] = new Vector3((float)roadData[i][j - 1][0] * scalingFactor, 0, (float)roadData[i][j - 1][1] * scalingFactor);
                        neighbours[1] = new Vector3((float)roadData[i][j + 1][0] * scalingFactor, 0, (float)roadData[i][j + 1][1] * scalingFactor);
                    }
                    if (PathFinder.nodeMap.ContainsKey(pos))
                        PathFinder.nodeMap[pos].addNeighbours(neighbours);
                    else
                        PathFinder.nodeMap[pos] = new Node(pos, neighbours, this.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
	
	}
}

using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

public class Data : MonoBehaviour {
    public int buildingsToGenerate = 100;
    public bool generateAll = false;
    public float scalingFactor = 20;

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

        for (int i = 0; i < (generateAll ? data.Count : buildingsToGenerate); i++) {
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
            if(i == 1117) { // Building 1117 caused the Visualizer class to crash in the extrude function
                Debug.Log("Vertices of Building 1117");
                foreach(Vector3 vertex in vertices)
                    Debug.Log(vertex);
            }
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

        List<List<double[]>> roadData = new List<List<double[]>>();
        roadData = parser.read("Highway.txt");
        for (int i = 0; i < roadData.Count; i++) {
            if (data[i].Count <= 2)
                continue;
            int j = 0;
            foreach (double[] dArray in data[i]) {
                if (j < data[i].Count - 1) { // Ignore last node, since it's equal to the first
                    Vector3 pos = new Vector3((float)dArray[0], 0, (float)dArray[1]);
                    if(PathFinder.nodeMap.ContainsKey(pos))
                //        PathFinder.nodeMap[pos].addNeighbours(
                    j++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
	
	}
}

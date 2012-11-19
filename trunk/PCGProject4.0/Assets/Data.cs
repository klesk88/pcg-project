using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

public class Data : MonoBehaviour {
    public int buildingsToGenerate = 100;
    public bool generateAll = false;


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
        //foreach(float f in data[3][3])
        //    Debug.Log(f);
        LSystem lsystem = new LSystem();

        for (int i = 0; i < (generateAll ? data.Count : buildingsToGenerate); i++) {
            Vector3[] vertices = new Vector3[data[i].Count];
            GameObject building = new GameObject();
            building.name = "Building_" + i;
            building.transform.parent = this.gameObject.transform;
            int j = 0;
            int height = 0;
            foreach (double[] dArray in data[i]) {
                if (j < data[i].Count - 1) { // Ignore last node, since it's equal to the first
                    vertices[j].x = (float)dArray[0];
                    vertices[j].z = (float)dArray[1];
                    if (dArray[2] != 0)
                        height = (int)dArray[2];
                    j++;
                }
            }
            if(height == 0)
                lsystem.visualize(building, vertices);
            else
                lsystem.visualize(building, vertices, height);
            if (i % 500 == 0 && i > 0)
                Debug.Log("Buildings created: " + i);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

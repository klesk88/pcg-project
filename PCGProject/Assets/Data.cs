using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

public class Data : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XMLParser parser = new XMLParser();
        parser.xmlParser();
        XMLParser.FileToCreate[] type_of_file = new XMLParser.FileToCreate[2];
        type_of_file[0] = XMLParser.FileToCreate.Buildings;
        type_of_file[1] = XMLParser.FileToCreate.Streets;
        parser.deleteAllFiles();
        parser.createFile(type_of_file);
        List<List<double[]>> data = new List<List<double[]>>(); 
        Debug.Log("before: " + data.Count);
        data = parser.read("Building.txt");
        Debug.Log("after: " + data.Count);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

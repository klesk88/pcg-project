using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

[ExecuteInEditMode()]

public class BuildingCreation : MonoBehaviour {

    public int buildingsToGenerate = 100;
    public bool generateAllBuildings = false;
    public float scalingFactor = 20;
    private List<List<double[]>> data;
    private XMLParser parser;
    public LSystem lsystem;

    public void readBuildings()
    {
        parser = new XMLParser();
        data = new List<List<double[]>>();
        data = parser.read("Building.txt");
    }

    public int getNumberOfBuildings()
    {
        return data.Count;
    }

	public void createBuilding()
    {
       
        /* parser.xmlParser();
         XMLParser.FileToCreate[] type_of_file = new XMLParser.FileToCreate[2];
         type_of_file[0] = XMLParser.FileToCreate.Buildings;
         type_of_file[1] = XMLParser.FileToCreate.Streets;
         parser.deleteAllFiles();
         parser.createFile(type_of_file);*/
        

        
        lsystem = this.gameObject.AddComponent<LSystem>();
    
        //    Debug.Log(f);
        //LSystem lsystem = new LSystem();
        //foreach(float f in data[3][3])
        lsystem.init();

        for (int i = 0; i < (generateAllBuildings ? data.Count : buildingsToGenerate); i++)
        {
            if (data[i].Count <= 2)
                continue;
            Vector3[] vertices = new Vector3[data[i].Count - 1];
            GameObject building = new GameObject();
            building.name = "Building_" + i;
            // building.AddComponent("CollisionDetection");
            //@Michele: add for reference in a simple way all the buildings in case of collision
            //building.tag = "Building";
            building.transform.parent = this.gameObject.transform;
            int j = 0;
            int height = 0;
            float minX = Mathf.Infinity, maxX = -Mathf.Infinity, minZ = Mathf.Infinity, maxZ = -Mathf.Infinity;
            foreach (double[] dArray in data[i])
            {
                if (j < data[i].Count - 1)
                { // Ignore last node, since it's equal to the first
                    //vertices[j].x = (float)dArray[0] * scalingFactor;
                    //vertices[j].z = (float)dArray[1] * scalingFactor;
                    vertices[j].x = (float)(dArray[0] * Terrain.activeTerrain.terrainData.size.x);// / (Terrain.activeTerrain.terrainData.size.x)) * (Terrain.activeTerrain.terrainData.heightmapResolution);
                    vertices[j].z = (float)(dArray[1] * Terrain.activeTerrain.terrainData.size.z);// / (Terrain.activeTerrain.terrainData.size.z)) * (Terrain.activeTerrain.terrainData.heightmapResolution);
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
            for (j = 0; j < vertices.Length; j++)
                vertices[j] -= offset;
            float randHeight = lsystem.randHeight();
            building.AddComponent<BoxCollider>();
            building.GetComponent<BoxCollider>().center = new Vector3(0, randHeight / 2, 0);
            building.GetComponent<BoxCollider>().size = new Vector3(maxX - minX, randHeight, maxZ - minZ);

            if (height == 0)
                lsystem.visualize(building/*this.gameObject*/, vertices, offset, randHeight);
            //   else
            //      lsystem.visualize(building, vertices, height);
            /*if (i % 100 == 0 && i > 0)
                Debug.Log("Buildings created: " + i);*/


            StreetCreation street = this.gameObject.AddComponent<StreetCreation>();
            street.getData();

        }
    }
}

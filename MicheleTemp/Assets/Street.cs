using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

public class Street : MonoBehaviour {

    List<List<double[]>> data;

	// Use this for initialization
	void Start () {

        XMLParser parser = new XMLParser();
        //parser.xmlParser();
       
        //parser.deleteAllFiles();
        //parser.createFile(type_of_file);
        data = new List<List<double[]>>();       
        data = parser.read("Highway.txt");
        //AttachedPathScript path = GetComponent <AttachedPathScript>();

        GameObject pathMesh = new GameObject();
        pathMesh.name = "Path";
        pathMesh.AddComponent(typeof(MeshFilter));
        pathMesh.AddComponent(typeof(MeshRenderer));
        pathMesh.AddComponent("AttachedPathScript");

   
        AttachedPathScript APS = (AttachedPathScript)pathMesh.GetComponent("AttachedPathScript");
        APS.pathMesh = pathMesh;
        APS.parentTerrain = gameObject;
        //APS.NewPath();


       Terrain terComponent = (Terrain)gameObject.GetComponent(typeof(Terrain));
     
       for(int i=0; i<1;i++)//data.Count; i++)
       {
           List<double[]> temp = data[i];
           APS.NewPath();
           APS.pathTexture = 1;
           APS.isRoad = true;
           //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
           //cube.AddComponent<Rigidbody>();
           //cube.transform.position = new Vector3(, y, 0);ù
           for(int j=0; j<temp.Count;j++)
           {
               Vector3 temp1;
               //the y is taken calculating the height of the terrain in that point
               temp1 = new Vector3((float)temp[j][0] * 10000000,Terrain.activeTerrain.SampleHeight(new Vector3((float)temp[j][0],(float)temp[j][1])), (float)temp[j][1] * 10000000);
              //temp1.x *= Terrain.activeTerrain.terrainData.size.x;
               //temp1.z *= Terrain.activeTerrain.terrainData.size.z;
               Debug.Log("size " + Terrain.activeTerrain.terrainData.size.x);
               temp1.x %= Terrain.activeTerrain.terrainData.size.x;
               temp1.z %= Terrain.activeTerrain.terrainData.size.z;
               //Debug.Log(Terrain.activeTerrain.terrainData.size.z + "ciao" +temp1.z);
               TerrainPathCell pathNodeCell = new TerrainPathCell();
               pathNodeCell.position.x = temp1.x;
               pathNodeCell.position.y = temp1.z;
               pathNodeCell.heightAtCell = temp1.y;

               APS.CreatePathNode(pathNodeCell);
               APS.addNodeMode = false;

               
           }
           //Debug.Log(APS.nodeObjects.Length);
          

           // define terrain cells
           APS.terrainCells = new TerrainPathCell[APS.terData.heightmapResolution * APS.terData.heightmapResolution];

           for (int x = 0; x < APS.terData.heightmapResolution; x++)
           {
               for (int y = 0; y < APS.terData.heightmapResolution; y++)
               {
                   APS.terrainCells[(y) + (x * APS.terData.heightmapResolution)].position.y = y;
                   APS.terrainCells[(y) + (x * APS.terData.heightmapResolution)].position.x = x;
                   APS.terrainCells[(y) + (x * APS.terData.heightmapResolution)].heightAtCell = APS.terrainHeights[y, x];
                   APS.terrainCells[(y) + (x * APS.terData.heightmapResolution)].isAdded = false;
               }
           }
         
           //APS.CreatePath(APS.pathSmooth, true, false);
           APS.pathMesh.renderer.enabled = true;
           APS.FinalizePath();
       }
	}

    // Update is called once per frame
    void Update()
    {
	    
        
	}
}

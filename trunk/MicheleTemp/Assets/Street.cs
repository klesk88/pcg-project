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


        //bool enter = false;
       Terrain terComponent = (Terrain)gameObject.GetComponent(typeof(Terrain));
       TerrainPathCell[] terrainCells = new TerrainPathCell[terComponent.terrainData.heightmapResolution * terComponent.terrainData.heightmapResolution]; ;
       float[,] terrainHeights = terComponent.terrainData.GetHeights(0, 0, terComponent.terrainData.heightmapResolution, terComponent.terrainData.heightmapResolution);
       for (int x = 0; x < terComponent.terrainData.heightmapResolution; x++)
       {
           for (int y = 0; y < terComponent.terrainData.heightmapResolution; y++)
           {
               terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].position.y = y;
               terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].position.x = x;
               terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].heightAtCell = terrainHeights[y, x];
               terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].isAdded = false;
           }
       }

       for(int i=0; i<10;i++)//data.Count; i++)
       {
           List<double[]> temp = data[i];
           GameObject pathMesh = new GameObject();
           pathMesh.name = "Path " + i;
           pathMesh.AddComponent(typeof(MeshFilter));
           pathMesh.AddComponent(typeof(MeshRenderer));
           pathMesh.AddComponent("AttachedPathScript");


           AttachedPathScript APS = (AttachedPathScript)pathMesh.GetComponent("AttachedPathScript");
           APS.pathMesh = pathMesh;
           APS.parentTerrain = gameObject;
           //APS.NewPath();
           APS.NewPath();
           APS.pathTexture = 1;
           APS.isRoad = true;
           APS.pathSmooth = 60;
           //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
           //cube.AddComponent<Rigidbody>();
           //cube.transform.position = new Vector3(, y, 0);ù
           for(int j=0; j<temp.Count;j++)
           {
               //Vector3 temp1;
               //the y is taken calculating the height of the terrain in t hat point
               //Debug.Log(Terrain.activeTerrain.terrainData.detailWidth + " " +Terrain.activeTerrain.terrainData.size.x);
               //temp1 = new Vector3((float)temp[j][0] * 10000000 , Terrain.activeTerrain.SampleHeight(new Vector3((float)temp[j][0], (float)temp[j][1])), (float)temp[j][1]);
               //Debug.Log(temp[j][0] * 1000000000000000);
              //temp1.x *= 5;
               //temp1.z *= Terrain.activeTerrain.terrainData.size.z;
               //Debug.Log("size " + Terrain.activeTerrain.terrainData.size.x);
               //temp1.x %= Terrain.activeTerrain.terrainData.size.x;
               //temp1.z %= Terrain.activeTerrain.terrainData.size.z;
               //Debug.Log(Terrain.activeTerrain.terrainData.size.z + "ciao" +temp1.z);
               TerrainPathCell pathNodeCell = new TerrainPathCell();
               pathNodeCell.position.x = Mathf.RoundToInt((float)((((temp[j][0] * 1000000000000000) % Terrain.activeTerrain.terrainData.size.x) / Terrain.activeTerrain.terrainData.size.x) * Terrain.activeTerrain.terrainData.heightmapResolution));
               pathNodeCell.position.y = Mathf.RoundToInt((float)((((temp[j][1] * 1000000000000000) % Terrain.activeTerrain.terrainData.size.z) / Terrain.activeTerrain.terrainData.size.z) * Terrain.activeTerrain.terrainData.heightmapResolution)); ;
               pathNodeCell.heightAtCell = Terrain.activeTerrain.SampleHeight(new Vector3(pathNodeCell.position.x, pathNodeCell.position.y));
               //Debug.Log("path node " + pathNodeCell.position);
               APS.CreatePathNode(pathNodeCell);
              

               
           }
           APS.addNodeMode = false;
           //Debug.Log(APS.nodeObjects.Length);
           /*
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
            * */
           // define terrain cells
           APS.terrainCells = new TerrainPathCell[APS.terData.heightmapResolution * APS.terData.heightmapResolution];

           //prova
           APS.terrainCells = terrainCells;
         
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

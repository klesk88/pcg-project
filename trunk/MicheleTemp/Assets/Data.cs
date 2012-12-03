using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

//[ExecuteInEditMode()]

public class Data : MonoBehaviour {
    public int buildingsToGenerate = 100;
    public bool generateAllBuildings = false;
    public float scalingFactor = 20;
    public bool readRoads = false;
    public int roadsToGenerate = 100;
    public bool generateAllRoads = false;

	// Use this for initialization
    public void Run() {
        XMLParser parser = new XMLParser();
        /* parser.xmlParser();
         XMLParser.FileToCreate[] type_of_file = new XMLParser.FileToCreate[2];
         type_of_file[0] = XMLParser.FileToCreate.Buildings;
         type_of_file[1] = XMLParser.FileToCreate.Streets;
         parser.deleteAllFiles();
         parser.createFile(type_of_file);*/
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
            if (data[i].Count <= 2)
                continue;
            Vector3[] vertices = new Vector3[data[i].Count - 1];
            GameObject building = new GameObject();
            building.name = "Building_" + i;
            building.transform.parent = this.gameObject.transform;
            int j = 0;
            int height = 0;
            float minX = Mathf.Infinity, maxX = -Mathf.Infinity, minZ = Mathf.Infinity, maxZ = -Mathf.Infinity;
            foreach (double[] dArray in data[i]) {
                if (j < data[i].Count - 1) { // Ignore last node, since it's equal to the first
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
            building.AddComponent("BoxCollider");
            building.GetComponent<BoxCollider>().center = new Vector3(0, randHeight / 2, 0);
            building.GetComponent<BoxCollider>().size = new Vector3(maxX - minX, randHeight, maxZ - minZ);

            if (height == 0)
                lsystem.visualize(building/*this.gameObject*/, vertices, offset, randHeight);
            //   else
            //      lsystem.visualize(building, vertices, height);
            /*if (i % 100 == 0 && i > 0)
                Debug.Log("Buildings created: " + i);*/
        }

        /*  if (readRoads) {
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
          }*/
        data = new List<List<double[]>>();
        data.Clear();
        data = parser.read("Highway.txt");
        Terrain terComponent = (Terrain)gameObject.GetComponent(typeof(Terrain));
        TerrainPathCell[] terrainCells = new TerrainPathCell[terComponent.terrainData.heightmapResolution * terComponent.terrainData.heightmapResolution]; ;
        float[,] terrainHeights = terComponent.terrainData.GetHeights(0, 0, terComponent.terrainData.heightmapResolution, terComponent.terrainData.heightmapResolution);
        for (int x = 0; x < terComponent.terrainData.heightmapResolution; x++) {
            for (int y = 0; y < terComponent.terrainData.heightmapResolution; y++) {
                terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].position.y = y;
                terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].position.x = x;
                terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].heightAtCell = terrainHeights[y, x];
                terrainCells[(y) + (x * terComponent.terrainData.heightmapResolution)].isAdded = false;
            }
        }


        for (int i = 0; i < data.Count; i++)
        {

            List<double[]> temp = data[i];
            //GameObject pathMesh = new GameObject();
            //pathMesh.name = "Path " + i;
            //pathMesh.AddComponent(typeof(MeshFilter));
            //pathMesh.AddComponent(typeof(MeshRenderer));
            //pathMesh.AddComponent("AttachedPathScript");


            //AttachedPathScript APS = (AttachedPathScript)pathMesh.GetComponent("AttachedPathScript");
            //APS.pathMesh = pathMesh;
            //APS.parentTerrain = gameObject;
            ////APS.NewPath();
            //APS.NewPath();
            //APS.pathTexture = 1;
            //APS.isRoad = false;
            //if(temp.Count > 2)
            //    APS.pathSmooth = 60;
            //else
            //    APS.pathSmooth = 5;
            //APS.pathUniform = true;
            //APS.pathWear = 0.5f;
            //bool check = false;


            for (int j = 0; j < temp.Count; j++) {
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
                //    TerrainPathCell pathNodeCell = new TerrainPathCell();
                //    pathNodeCell.position.x = Mathf.RoundToInt((float)((((temp[j][0] * /*1000000000000000) % */Terrain.activeTerrain.terrainData.size.x)) / Terrain.activeTerrain.terrainData.size.x) * (Terrain.activeTerrain.terrainData.heightmapResolution)));
                //    pathNodeCell.position.y = Mathf.RoundToInt((float)((((temp[j][1] * /*1000000000000000) % */Terrain.activeTerrain.terrainData.size.z)) / Terrain.activeTerrain.terrainData.size.z) * (Terrain.activeTerrain.terrainData.heightmapResolution)));

                //    pathNodeCell.heightAtCell = (Terrain.activeTerrain.SampleHeight(new Vector3(pathNodeCell.position.x, pathNodeCell.position.y))) / Terrain.activeTerrain.terrainData.size.y;
                //    //Debug.Log(pathNodeCell.heightAtCell);
                //    //Debug.Log("path node " + pathNodeCell.position);

                //    if (!APS.CreatePathNode(pathNodeCell))
                //    {
                //        check = true;
                //        break;
                //    }



                //}

                //if (check)
                //{
                //    DestroyImmediate(pathMesh);
                //    continue;
                //}

                // -- Pathfinding --

                //for (int h=0; h<APS.nodeObjects.Length; h++)
                //{
                //Vector3 pos = new Vector3((float)temp[j][0], 0f, (float)temp[j][1]);
                //Vector3[] neighbours;
                //if (j == 0)
                //    neighbours = new Vector3[0];
                //else
                //{
                //    neighbours = new Vector3[1];
                //    neighbours[0] = APS.nodeObjects[h - 1].position;
                //    PathFinder.nodeMap[APS.nodeObjects[h - 1].position].addNeighbours(new Vector3[] { pos });
                //}
                //if (PathFinder.nodeMap.ContainsKey(pos))
                //    PathFinder.nodeMap[pos].addNeighbours(neighbours);
                //else
                //    PathFinder.nodeMap[pos] = new Node(pos, neighbours, pathMesh);

                Vector3 pos = new Vector3((float)temp[j][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j][1] * Terrain.activeTerrain.terrainData.size.z);
                Vector3[] neighbours;
                if (j == 0) {
                    neighbours = new Vector3[1];
                    neighbours[0] = new Vector3((float)temp[j + 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j + 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                else if (j == temp.Count - 1) {
                    neighbours = new Vector3[1];
                    neighbours[0] = new Vector3((float)temp[j - 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j - 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                else {
                    neighbours = new Vector3[2];
                    neighbours[0] = new Vector3((float)temp[j - 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j - 1][1] * Terrain.activeTerrain.terrainData.size.z);
                    neighbours[1] = new Vector3((float)temp[j + 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j + 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                if (PathFinder.nodeMap.ContainsKey(pos))
                    PathFinder.nodeMap[pos].addNeighbours(neighbours);
                else
                    PathFinder.nodeMap[pos] = new Node(pos, neighbours, this.gameObject);
                //  }
                //  APS.addNodeMode = false;
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
                //APS.terrainCells = new TerrainPathCell[APS.terData.heightmapResolution * APS.terData.heightmapResolution];

                ////prova
                //APS.terrainCells = terrainCells;

                ////APS.CreatePath(APS.pathSmooth, true, false);
                ////APS.pathMesh.renderer.enabled = true;
                ////APS.pathMesh.renderer.material.color = Color.grey;
                //APS.FinalizePath();
                //APS.pathMesh.renderer.enabled = true;
                //APS.pathMesh.renderer.material.color = Color.grey;

                // APS.pathMesh.renderer.material.mainTexture = (Texture)Resources.Load("Grass (Hill)");
                // APS.pathMesh.renderer.sharedMaterial.mainTexture = texture;
            }
        }
    }

    // Update is called once per frame
    void Update() {
	
	}
}

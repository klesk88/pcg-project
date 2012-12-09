using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XMLParserUnity;

//[ExecuteInEditMode()]

public class StreetCreation : MonoBehaviour {
    
    public bool readRoads = false;
    public int roadsToGenerate = 100;
    public bool generateAllRoads = false;

	public void getData()
    {
        XMLParser parser = new XMLParser();
        List<List<double[]>>  data = new List<List<double[]>>();
        data.Clear();
        data = parser.read("Highway.txt");
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


        for (int i = 0; i < data.Count; i++)
        {

            List<double[]> temp = data[i];
          

            for (int j = 0; j < temp.Count; j++)
            {

                Vector3 pos = new Vector3((float)temp[j][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j][1] * Terrain.activeTerrain.terrainData.size.z);
                Vector3[] neighbours;
                if (j == 0)
                {
                    neighbours = new Vector3[1];
                    neighbours[0] = new Vector3((float)temp[j + 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j + 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                else if (j == temp.Count - 1)
                {
                    neighbours = new Vector3[1];
                    neighbours[0] = new Vector3((float)temp[j - 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j - 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                else
                {
                    neighbours = new Vector3[2];
                    neighbours[0] = new Vector3((float)temp[j - 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j - 1][1] * Terrain.activeTerrain.terrainData.size.z);
                    neighbours[1] = new Vector3((float)temp[j + 1][0] * Terrain.activeTerrain.terrainData.size.x, 0, (float)temp[j + 1][1] * Terrain.activeTerrain.terrainData.size.z);
                }
                if (PathFinder.nodeMap.ContainsKey(pos))
                    PathFinder.nodeMap[pos].addNeighbours(neighbours);
                else
                    PathFinder.nodeMap[pos] = new Node(pos, neighbours, this.gameObject);
              
            }
        }
    }

    public void createStreets()
    {
        
    }
}

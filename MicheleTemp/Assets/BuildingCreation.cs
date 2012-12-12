using UnityEngine;
using System.Collections;
using XMLParserUnity;
using System.Collections.Generic;

[ExecuteInEditMode()]

public class BuildingCreation : MonoBehaviour {

    public int buildingsToGenerate = 100;
    public bool generateAllBuildings = true;
    public float scalingFactor = 20;
    private List<List<double[]>> data;
    private XMLParser parser;
 

    // LSystem
    //public bool moveToParentTransform = false;
    //public bool useOldVisualizerAndState = true;
    public string axiom = "E";
    public string productionRule = "EyE[xz]";
    public string exampleRule1 = "EyE[xz]";
    public int expansions = 2;
    public float height = 5, heightMin = 0.5f, heightMax = 5.0f;
    Dictionary<string, string> productionRules = new Dictionary<string, string>();
    bool initialized = false;

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
        

        
    
        //    Debug.Log(f);
        //LSystem lsystem = new LSystem();
        //foreach(float f in data[3][3])
        init();
        Debug.Log(data.Count);


        GameObject buildings = new GameObject();
        buildings.name = "Buildings";
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
            building.transform.parent = /*this*/buildings.gameObject.transform;
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
            float m_randHeight = randHeight();
            building.AddComponent<BoxCollider>();
            building.GetComponent<BoxCollider>().center = new Vector3(0, m_randHeight / 2, 0);
            building.GetComponent<BoxCollider>().size = new Vector3(maxX - minX, m_randHeight, maxZ - minZ);

            if (height == 0)
                visualize(building/*this.gameObject*/, vertices, offset, m_randHeight);
            //   else
            //      lsystem.visualize(building, vertices, height);
            /*if (i % 100 == 0 && i > 0)
                Debug.Log("Buildings created: " + i);*/



        }


        StreetCreation street = this.gameObject.AddComponent<StreetCreation>();
        street.getData();
    }
    private void expand(int depth) {
        char[] axiomSplit;
        axiomSplit = new char[1];
        axiomSplit[0] = 'E';
        Dictionary<string, string>.KeyCollection coll = productionRules.Keys;
        string premise = "";
        foreach (string s in coll)
            premise = s;
        for (int i = 0; i < depth; i++) {
            if (i > 0) {
                axiomSplit = axiom.ToCharArray();
                //Debug.Log("axiomSplitLength: " + axiomSplit.Length);
            }
            axiom = "";
            for (int j = 0; j < axiomSplit.Length; j++) {
                if (axiomSplit[j].Equals(premise.ToCharArray()[0]))
                    axiom += productionRules[premise];
                else
                    axiom += axiomSplit[j];
            }
        }
    }

    private void visualize(GameObject gameObject, Vector3[] groundVertices, Vector3 offset, float height = 10) {

            Stack<StateOld> StateStack = new Stack<StateOld>();
            StateOld currentState = new StateOld(gameObject, groundVertices);
            for (int i = 0; i < axiom.Length; i++) {
                char symbol = axiom[i];
                if (symbol != '[' && symbol != ']') {
                    if (symbol == 'E')
                        VisualizerOld.extrude(currentState, height);
                    else if (symbol == 'x')
                        VisualizerOld.scale(currentState, 0.6f, 1, 1);
                    else if (symbol == 'y')
                        VisualizerOld.scale(currentState, 1, 0.4f, 1);
                    else if (symbol == 'z')
                        VisualizerOld.scale(currentState, 1, 1, 0.9f);
                    else if (symbol == 'X')
                        VisualizerOld.translate(currentState, 0.2f, 0, 0);
                    else if (symbol == 'Y')
                        VisualizerOld.translate(currentState, 0, -1, 0);
                    else if (symbol == 'Z')
                        VisualizerOld.translate(currentState, 0, 0, 0.2f);

                }
                else if (axiom[i] == '[')
                    StateStack.Push(currentState.clone());
                else if (axiom[i] == ']')
                    currentState = StateStack.Pop();
            }
        
        if (offset != null) {
            gameObject.transform.Translate(offset);
        }
    }

    //public LSystem getInstance() { return lsystem; }

    private float randHeight() {
        return Random.Range(heightMin, heightMax);
    }

    void init() {
        if (!initialized) {
            productionRules.Add(axiom, productionRule);
            expand(expansions);
            initialized = true;
        }
    }
}

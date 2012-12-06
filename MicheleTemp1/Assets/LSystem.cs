using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour {
    public string UseExample = "cShape";
    public bool moveToParentTransform = false;
    public bool useOldVisualizerAndState = true;
    public string axiom = "E";
    public string productionRule = "EyE[xz]";
    public string exampleRule1 = "EyE[xz]";
    public int expansions = 2;
    public float height = 5, heightMin = 0.5f, heightMax = 5.0f;
    Dictionary<string, string> productionRules = new Dictionary<string, string>();
    bool initialized = false;

    // I've commented the constructor out, since it caused Unity to stop creating an object of the class by default
    /*public LSystem(string rule = "", int expansions = 2) {
        string rule1 = "Ey[Ex]", rule2 = "EyEx", rule3 = "EyE[xz]";
        if (rule == "")
            rule = rule3;
        productionRules.Add("E", rule);
        expand(expansions);
    }*/

    public void expand(int depth) {
        char[] axiomSplit;
        axiomSplit = new char[1];
        axiomSplit[0] = 'E';    
        Dictionary<string, string>.KeyCollection coll = productionRules.Keys;
        string premise = "";
        foreach(string s in coll)
            premise = s;
        for(int i = 0; i < depth; i++) {
            if(i > 0) {
                axiomSplit = axiom.ToCharArray();
                //Debug.Log("axiomSplitLength: " + axiomSplit.Length);
            }
            axiom = "";
            for(int j = 0; j < axiomSplit.Length; j++) {
                if(axiomSplit[j].Equals(premise.ToCharArray()[0]))
                    axiom += productionRules[premise];
                else
                    axiom += axiomSplit[j];
            }
        }
    }

    public void init() { Start(); }

    public void visualize(GameObject gameObject, Vector3[] groundVertices, Vector3 offset, float height = 10) {
        if(!useOldVisualizerAndState) {
            Stack<State> StateStack = new Stack<State>();
            State currentState = new State(gameObject, groundVertices);
            for(int i = 0; i < axiom.Length; i++) {
                char symbol = axiom[i];
                if(symbol != '[' && symbol != ']') {
                    if(symbol == 'E') {
                        //  Debug.Log("Y PRE EXTRUDE: " + currentState.getTop()[0].y);
                        Visualizer.extrude(currentState, height);
                        //  Debug.Log("Y POST EXTRUDE: " + currentState.getTop()[0].y);
                    }
                    else if(symbol == 'x')
                        Visualizer.scale(currentState, new Vector3(0.6f, 1, 1));
                    else if(symbol == 'y') {
                        // Debug.Log("Y PRE SCALE: " + currentState.getTop()[0].y);
                        Visualizer.scale(currentState, new Vector3(1, 0.4f, 1));
                        // Debug.Log("Y POST SCALE: " + currentState.getTop()[0].y);
                    }
                    else if(symbol == 'z')
                        Visualizer.scale(currentState, new Vector3(1, 1, 0.9f));
                    else if(symbol == 'X')
                        Visualizer.translate(currentState, new Vector3(1, 0, 0));
                    else if(symbol == 'Y') {
                        //  Debug.Log("Y PRE TRANSLATE: " + currentState.getTop()[0].y);
                        Visualizer.translate(currentState, new Vector3(0, -3, 0));
                        //  Debug.Log("Y POST TRANSLATE: " + currentState.getTop()[0].y);
                    }
                    else if(symbol == 'Z')
                        Visualizer.translate(currentState, new Vector3(0, 0, 0.2f));

                }
                else if(axiom[i] == '[')
                    StateStack.Push(currentState.clone());
                else if(axiom[i] == ']')
                    currentState = StateStack.Pop();
            }
        }
        else {
            Stack<StateOld> StateStack = new Stack<StateOld>();
            StateOld currentState = new StateOld(gameObject, groundVertices);
            for(int i = 0; i < axiom.Length; i++) {
                char symbol = axiom[i];
                if(symbol != '[' && symbol != ']') {
                    if(symbol == 'E')
                        VisualizerOld.extrude(currentState, height);
                    else if(symbol == 'x')
                        VisualizerOld.scale(currentState, 0.6f, 1, 1);
                    else if(symbol == 'y')
                        VisualizerOld.scale(currentState, 1, 0.4f, 1);
                    else if(symbol == 'z')
                        VisualizerOld.scale(currentState, 1, 1, 0.9f);
                    else if(symbol == 'X')
                        VisualizerOld.translate(currentState, 0.2f, 0, 0);
                    else if(symbol == 'Y')
                        VisualizerOld.translate(currentState, 0, -1, 0);
                    else if(symbol == 'Z')
                        VisualizerOld.translate(currentState, 0, 0, 0.2f);

                }
                else if(axiom[i] == '[')
                    StateStack.Push(currentState.clone());
                else if(axiom[i] == ']')
                    currentState = StateStack.Pop();
            }
        }
        if(offset != null) {
            gameObject.transform.Translate(offset);
        }
    }

    //public LSystem getInstance() { return lsystem; }

    public float randHeight() {
        return Random.Range(heightMin, heightMax);
    }
    
    
    void Start () {
        if(!initialized) {
            productionRules.Add(axiom, productionRule);
            expand(expansions);
            if(UseExample == "cShape") {
                Vector3[] cShape = new Vector3[9];
                cShape[0].x = -2; cShape[0].z = 0;
                cShape[1].x = -0.5f; cShape[1].z = 1;
                cShape[2].x = 0; cShape[2].z = 1;
                cShape[3].x = 3; cShape[3].z = -1;
                cShape[4].x = 1; cShape[4].z = -3;
                cShape[5].x = 0; cShape[5].z = -2.3f;
                cShape[6].x = 1; cShape[6].z = -1.6f;
                cShape[7].x = -0.5f; cShape[7].z = -0.7f;
                cShape[8].x = -1.2f; cShape[8].z = -1;

                /*     cShape[0].x = 0.582927f; cShape[0].z = 0.66314f;
                     cShape[1].x = 0.651016f; cShape[1].z = 0.35805f;
                     cShape[2].x = 0.168902f; cShape[2].z = 0.20932f;
                     cShape[3].x = 0.042073f; cShape[3].z = 0.77754f;
                     cShape[4].x = 0.525407f; cShape[4].z = 0.9267f;
                     cShape[5].x = 0.54309f; cShape[5].z = 0.84788f;
                     cShape[6].x = 0.626016f; cShape[6].z = 0.87373f;
                     cShape[7].x = 8.667276f; cShape[7].z = 51.6894f;*/

                //Debug.Log("CShape is being visualized!");
                visualize(this.gameObject, cShape, Vector3.zero, 10);
            }
            if(moveToParentTransform) {
                for(int i = 0; i < this.gameObject.transform.GetChildCount(); i++)
                    this.gameObject.transform.GetChild(i).localPosition = new Vector3(0, 0, 0);
            }
            initialized = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

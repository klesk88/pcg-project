using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour {
    public string UseExample = "cShape";
    public bool moveToParentTransform = false;
    string axiom;
    Dictionary<string, string> productionRule = new Dictionary<string, string>();

    public LSystem(string rule = "", int expansions = 2) {
        axiom = "E";
        string rule1 = "Ey[Ex]", rule2 = "EyEx", rule3 = "EyE[xz]";
        if (rule == "")
            rule = rule3;
        productionRule.Add("E", rule);
        expand(expansions);
    }

    public void expand(int depth) {
        char[] axiomSplit;
        axiomSplit = new char[1];
        axiomSplit[0] = 'E';
        Dictionary<string, string>.KeyCollection coll = productionRule.Keys;
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
                    axiom += productionRule[premise];
                else
                    axiom += axiomSplit[j];
            }
        }
    }

    public void visualize(GameObject gameObject, Vector3[] groundVertices, int height = 10) {
        Stack<State> StateStack = new Stack<State>();
        State currentState = new State(gameObject, groundVertices);
        for(int i = 0; i < axiom.Length; i++) {
            char symbol = axiom[i];
            if(symbol != '[' && symbol != ']') {
                if(symbol == 'E')
                    Visualizer.extrude(currentState, height);
                else if(symbol == 'x')
                    Visualizer.scale(currentState, 0.6f, 1, 1);
                else if(symbol == 'y')
                    Visualizer.scale(currentState, 1, 0.4f, 1);
                else if(symbol == 'z')
                    Visualizer.scale(currentState, 1, 1, 0.9f);
                else if(symbol == 'X')
                    Visualizer.translate(currentState, 0.2f, 0, 0);
                else if(symbol == 'Y')
                    Visualizer.translate(currentState, 0, -1, 0);
                else if(symbol == 'Z')
                    Visualizer.translate(currentState, 0, 0, 0.2f);
                
            }
            else if(axiom[i] == '[')
                StateStack.Push(currentState.clone());
            else if(axiom[i] == ']')
                currentState = StateStack.Pop();
        }
    }
    
    
    
    void Start () {
        LSystem lsystem = new LSystem();
        if (UseExample == "cShape") {
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
            Debug.Log("CShape is being visualized!");
            lsystem.visualize(this.gameObject, cShape);
        }
        if (moveToParentTransform) {
            for (int i = 0; i < this.gameObject.transform.GetChildCount(); i++)
                this.gameObject.transform.GetChild(i).localPosition = new Vector3(0, 0, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

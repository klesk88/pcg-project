using UnityEngine;
using System.Collections;

public class CityBuilder : MonoBehaviour{
	public static int buildingNumber = 1;
	void Start(){
        Vector3[] exampleVertices = new Vector3[5];
        exampleVertices[0].x = -2; exampleVertices[0].y = 0; exampleVertices[0].z = -2;
        exampleVertices[1].x = -2; exampleVertices[1].y = 0; exampleVertices[1].z = 1.5f;
        exampleVertices[2].x = 1.5f; exampleVertices[2].y = 0; exampleVertices[2].z = 2;
        exampleVertices[3].x = 2; exampleVertices[3].y = 0; exampleVertices[3].z = -2;
        exampleVertices[4].x = 3; exampleVertices[4].y = 0; exampleVertices[4].z = -3;
        addBuilding(exampleVertices, 5);
	}
	static void addBuilding(Vector3[] groundVertices, int height = 7, Vector2[] uvs = null, int[] triangles = null, Vector3[] normals = null){
		GameObject building = new GameObject("Building_" + buildingNumber);
		building.AddComponent("MeshFilter");
        building.AddComponent("MeshRenderer");
        building.GetComponent<MeshRenderer>().material.color = Color.white;
		Mesh mesh = building.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = new Vector3[2*groundVertices.Length];
		
		for(int i = 0; i < vertices.Length; i++){
			if(i < groundVertices.Length)
				vertices[i] = groundVertices[i];
			else{
				vertices[i] = groundVertices[i-groundVertices.Length];
				vertices[i].y += height;
			}
		}	//string test = "44.33";		float.TryParse(test, System.Globalization.NumberStyles.
		if(uvs == null)
			uvs = new Vector2[vertices.Length];
		if(triangles == null){
			// 8 vertices: 5 sides = 10 triangles
			// 10 vertices: 5 sides + topSide = 13 triangles
			// 12 vertices: 6 sides + topSide = 16 triangles
			// 14 vertices: 7 sides + topSide = 19 triangles ==>
			triangles = new int[(groundVertices.Length*2 + groundVertices.Length - 2) * 3];
			// 8 vertices: tri1 = 0,1,4; tri2 = 1,5,4; tri3 = 1,2,5 tri4 = 2,6,5 ... .. 0 -> vertices.length-1: vertical triangles
			int tris = 0;
			for(int i = 0; i < vertices.Length * 3; i += 6){
                if(i < (vertices.Length - 2) * 3){
				    triangles[i] = tris; triangles[i+1] = tris + 1; triangles[i+2] = tris + 4;
				    triangles[i+3] = tris + 1; triangles[i+4] = tris + 5; triangles[i+5] = tris + 4;
                }
                else{
                    triangles[i] = tris; triangles[i+1] = 0; triangles[i+2] = tris + 4;
				    triangles[i+3] = 0; triangles[i+4] = tris + 1; triangles[i+5] = tris + 4;
                }
				tris++;
			}
			// 8 vertices  topTris: topTri1 = 4,5,6; topTri2 = 6,7,4 ... .. vertices.length -> end: top triangles
			// 10 vertices topTris: topTri1 = 5,6,7; topTri2 = 7,8,5; topTri3 = 8,9,5
			// 12 vertices topTris: topTri1 = 6,7,8; topTri2 = 8,9,6; topTri3 = 9,10,6; topTri4 = 10,11,6
			tris = groundVertices.Length;
			for(int i = vertices.Length * 3; i < triangles.Length; i += 3){
				if(i == vertices.Length * 3){
					triangles[i] = tris; triangles[i+1] = tris + 1; triangles[i+2] = tris + 2;
				}
				else{
                    triangles[i] = tris + 1; triangles[i+1] = tris + 2; triangles[i+2] = groundVertices.Length;
				}
				tris++;
			}
		}
        if(normals == null){
            normals = new Vector3[vertices.Length];
            float xSum = 0, ySum = 0, zSum = 0;
            for(int i = 0; i < vertices.Length; i++){
                xSum += vertices[i].x; ySum += vertices[i].y; zSum += vertices[i].z;
            }
            Vector3 meanCenter = new Vector3(xSum / vertices.Length, ySum / vertices.Length, zSum / vertices.Length);
            for (int i = 0; i < normals.Length; i++)
                normals[i] = (vertices[i] - meanCenter).normalized;
        }
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
        for(int i = 0; i < triangles.Length; i++)
            Debug.Log(i + ": " + triangles[i]);
        mesh.triangles = triangles;
        mesh.normals = normals;
        buildingNumber++;
	}
}
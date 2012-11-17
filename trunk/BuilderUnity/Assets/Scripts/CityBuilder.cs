using UnityEngine;
using System.Collections;

public class CityBuilder : MonoBehaviour {
    public static int buildingNumber = 1;
    void Start() {
        Vector3[] exampleVertices = new Vector3[8];
        exampleVertices[0].x = -2; exampleVertices[0].z = -2;
        exampleVertices[1].x = -2; exampleVertices[1].z = 1.5f;
        exampleVertices[2].x = 0.5f; exampleVertices[2].z = 2.5f;
        exampleVertices[3].x = 2; exampleVertices[3].z = 2;
        exampleVertices[4].x = 2; exampleVertices[4].z = -2;
        exampleVertices[5].x = 1.7f; exampleVertices[5].z = -6;
        exampleVertices[6].x = 0.7f; exampleVertices[6].z = -4;
        exampleVertices[7].x = -1.2f; exampleVertices[7].z = -2.1f;
        addBuilding(exampleVertices, 3);
    }
    static void addBuilding(Vector3[] groundVertices, int height = 10, Vector2[] uvs = null, int[] triangles = null, Vector3[] normals = null) {
        GameObject building = new GameObject("Building_" + buildingNumber);
        building.AddComponent("MeshFilter");
        building.AddComponent("MeshRenderer");
        building.GetComponent<MeshRenderer>().material.color = Color.white;
        Mesh mesh = building.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[2 * groundVertices.Length];

        int vBot = groundVertices.Length, vTotal = vertices.Length;
        for(int i = 0; i < vertices.Length; i++) {
            if(i < vBot)
                vertices[i] = groundVertices[i];
            else {
                vertices[i] = groundVertices[i - vBot];
                vertices[i].y += height;
            }
        }
        if(uvs == null)
            uvs = new Vector2[vTotal];
        if(triangles == null) {
            // 8 vertices: 4 sides + topSide = 10 triangles
            // 10 vertices: 5 sides + topSide = 13 triangles
            // 12 vertices: 6 sides + topSide = 16 triangles
            // 14 vertices: 7 sides + topSide = 19 triangles ==>
            triangles = new int[(vBot * 2 + vBot - 2) * 3];
            // 8 vertices: tri1 = 0,1,4; tri2 = 1,5,4; tri3 = 1,2,5 tri4 = 2,6,5 ... .. 0 -> vertices.length-1: vertical triangles
            int tris = 0;
            for(int i = 0; i < vTotal * 3; i += 6) {
                if(i < (vTotal - 2) * 3) {
                    triangles[i] = tris; triangles[i + 1] = tris + 1; triangles[i + 2] = tris + vBot;
                    triangles[i + 3] = tris + 1; triangles[i + 4] = tris + 1 + vBot; triangles[i + 5] = tris + vBot;
                    Debug.Log("FirstIF");
                }
                else {
                    triangles[i] = tris; triangles[i + 1] = 0; triangles[i + 2] = tris + vBot;
                    triangles[i + 3] = 0; triangles[i + 4] = tris + 1; triangles[i + 5] = tris + vBot;
                    Debug.Log("SecondIF");
                }
                tris++;
            }
            // 8 vertices  topTris: topTri1 = 4,5,6; topTri2 = 6,7,4 ... .. vertices.length -> end: top triangles
            // 10 vertices topTris: topTri1 = 5,6,7; topTri2 = 7,8,5; topTri3 = 8,9,5
            // 12 vertices topTris: topTri1 = 6,7,8; topTri2 = 8,9,6; topTri3 = 9,10,6; topTri4 = 10,11,6
            tris = vBot;
            for(int i = vTotal * 3; i < triangles.Length; i += 3) {
                if(i == vTotal * 3) {
                    triangles[i] = tris; triangles[i + 1] = tris + 1; triangles[i + 2] = tris + 2;
                }
                else {
                    // triangles[i] = tris + 1; triangles[i + 1] = tris + 2; triangles[i + 2] = vBot; Backup - old method of roof constructing
                    //if(tris + 3 > vTotal) {
                        triangles[i] = tris + 1; triangles[i + 1] = tris + 2; triangles[i + 2] = vBot;
                   /* }
                    else {
                        triangles[i] = tris + 1; triangles[i + 1] = tris + 2; triangles[i + 2] = tris + 3;
                        triangles[i + 3] = tris + 1; triangles[i + 4] = tris + 3; triangles[i + 5] = vBot;
                        //tris++;
                        i += 3;
                    }*/
                }
                tris++;
            }
        }
        if(normals == null) {
            normals = new Vector3[vTotal];
            float xSum = 0, ySum = 0, zSum = 0;
            for(int i = 0; i < vertices.Length; i++) {
                xSum += vertices[i].x; ySum += vertices[i].y; zSum += vertices[i].z;
            }
            Vector3 meanCenter = new Vector3(xSum / vTotal, ySum / vertices.Length, zSum / vTotal);
            for(int i = 0; i < normals.Length; i++)
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
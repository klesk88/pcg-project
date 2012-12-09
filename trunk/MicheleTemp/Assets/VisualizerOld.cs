using UnityEngine;
using System.Collections;

public class VisualizerOld : MonoBehaviour {
    static int extrusionNumber = 1;

    public static void extrude(StateOld state, float height = 10, Vector2[] uvs = null, int[] triangles = null, Vector3[] normals = null) {
        //Debug.Log("Extrusion called on " + state.getObject().name);
        GameObject extrusion = new GameObject("Extrusion_" + extrusionNumber);
        extrusion.transform.parent = state.getObject().transform;
        extrusion.AddComponent("MeshFilter");
        extrusion.AddComponent("MeshRenderer");
        extrusion.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
        extrusion.GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
        //extrusion.GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
        Mesh mesh = extrusion.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
            extrusion.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        else
            mesh.Clear();
        Vector3[] groundVertices = state.getFace();
        Vector3[] vertices = new Vector3[2 * groundVertices.Length];
        Vector3[] topVertices = new Vector3[groundVertices.Length];

        int vBot = groundVertices.Length, vTotal = vertices.Length;
        for(int i = 0; i < vertices.Length; i++) {
            if(i < vBot)
                vertices[i] = groundVertices[i];
            else {
                vertices[i] = groundVertices[i - vBot];
                vertices[i].y += height;
                topVertices[i - vBot] = vertices[i];
            }
        }
        if(uvs == null)
            uvs = new Vector2[vTotal];
        if(triangles == null) {
            triangles = new int[(vBot * 2 + vBot - 2) * 3];
            int tris = 0;
            for(int i = 0; i < vTotal * 3; i += 6) {
                if(i < (vTotal - 2) * 3) {
                    triangles[i] = tris; triangles[i + 1] = tris + 1; triangles[i + 2] = tris + vBot;
                    triangles[i + 3] = tris + 1; triangles[i + 4] = tris + 1 + vBot; triangles[i + 5] = tris + vBot;
                }
                else {
                    triangles[i] = tris; triangles[i + 1] = 0; triangles[i + 2] = tris + vBot;
                    triangles[i + 3] = 0; triangles[i + 4] = tris + 1; triangles[i + 5] = tris + vBot;
                }
                tris++;
            }
            tris = vBot;
            for(int i = vTotal * 3; i < triangles.Length; i += 3) {
                if(i == vTotal * 3) {
                    triangles[i] = tris; triangles[i + 1] = tris + 1; triangles[i + 2] = tris + 2;
                }
                else {
                    triangles[i] = tris + 1; triangles[i + 1] = tris + 2; triangles[i + 2] = vBot;
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
        mesh.triangles = triangles;
        mesh.normals = normals;
        extrusionNumber++;
        state.setObject(extrusion);
        state.setFace(topVertices);
    }

    public static void scale(StateOld state, float x = 1, float y = 1, float z = 1) {
        state.getObject().transform.localScale = Vector3.Scale(state.getObject().transform.localScale, new Vector3(x, y, z));
        Vector3[] face = state.getFace();
        for(int i = 0; i < face.Length; i++) {
            face[i].x *= x;
            face[i].y *= y;
            face[i].z *= z;
        }
        state.setFace(face);
    }

    public static void translate(StateOld state, float x = 0, float y = 0, float z = 0) {
        state.getObject().transform.Translate(new Vector3(x, y, z));
        Vector3[] face = state.getFace();
        for(int i = 0; i < face.Length; i++) {
            face[i].x += x;
            face[i].y += y;
            face[i].z += z;
        }
        state.setFace(face);
    }
}
using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour {
    static int extrusionNumber = 1;

    public static void extrude(State state, float height = 10, Vector2[] uvs = null, int[] triangles = null, Vector3[] normals = null) {
        GameObject extrusion = new GameObject("Extrusion_" + extrusionNumber);
        //extrusion.transform.position = Vector3.zero;
        //extrusion.transform.localPosition = Vector3.zero;
        extrusion.AddComponent("MeshFilter");
        extrusion.AddComponent("MeshRenderer");
        extrusion.GetComponent<MeshRenderer>().material.color = Color.white;
        Mesh mesh = extrusion.GetComponent<MeshFilter>().mesh;
        Vector3[] groundVertices = state.getTop();
        Debug.Log("groundVerex0.y: " + groundVertices[0].y);
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
        float maxX = vertices[0].x, minX = vertices[0].x, maxZ = vertices[0].z, minZ = vertices[0].z;
        if(normals == null) {
            normals = new Vector3[vTotal];
            float xSum = 0, ySum = 0, zSum = 0; 
            for(int i = 0; i < vertices.Length; i++) {
                xSum += vertices[i].x; ySum += vertices[i].y; zSum += vertices[i].z;
                if(vertices[i].x < minX)
                    minX = vertices[i].x;
                if(vertices[i].x > maxX)
                    maxX = vertices[i].x;
                if(vertices[i].z < minZ)
                    minZ = vertices[i].z;
                if(vertices[i].z > maxZ)
                    maxZ = vertices[i].z;
            }
            Vector3 meanCenter = new Vector3(xSum / vTotal, ySum / vertices.Length, zSum / vTotal);
            for(int i = 0; i < normals.Length; i++)
                normals[i] = (vertices[i] - meanCenter).normalized;
        }
        mesh.bounds = new Bounds(new Vector3(minX + ((maxX - minX) / 2), groundVertices[0].y + height / 2, minZ + ((maxZ - minZ) / 2)), new Vector3(maxX - minX, height, maxZ - minZ));

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.normals = normals;
        extrusionNumber++;
        //mesh.RecalculateBounds();
       

        //Debug.Log("deltaPos.y: " + deltaPos.y);

        //extrusion.transform.position += deltaPos;

     //   extrusion.transform.position += deltaPos;
       // for(int i = 0; i < topVertices.Length; i++)
          //  topVertices[i] += 0;

  /*      Vector3[] newVerts = mesh.vertices;
        Vector3[] newTops = new Vector3[newVerts.Length / 2];
        for(int i = 0; i < newTops.Length; i++) {
            newTops[i] = newVerts[i + newTops.Length] - deltaPos;
            //Debug.Log(newTops[i].y);
        }*/
        extrusion.transform.parent = state.getObject().transform;
        Debug.Log("transform BEFORE pivotoffset: " + extrusion.transform.position);

        extrusion.transform.Translate(new Vector3(state.getPivotOffset().x * state.getObject().transform.localScale.x,
            state.getPivotOffset().y * state.getObject().transform.localScale.y, 
            state.getPivotOffset().z * state.getObject().transform.localScale.z));

        Debug.Log("getPivot fra extrusion(): " + state.getPivotOffset());
        Debug.Log("transform AFTER pivotoffset: " + extrusion.transform.position);
        state.setObject(extrusion);
        //state.setFace(newTops);
        
    }

    public static Vector3 centerPivot(GameObject meshObject){
     // This should recenter the pivot to prevent scaling issues
        Transform[] children = new Transform[meshObject.transform.childCount];
        for(int i = 0; i < children.Length; i++)
            children[i] = meshObject.transform.GetChild(i);
        meshObject.transform.DetachChildren();
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        Vector3 offset = -1 * bounds.center;
        Vector3 extent = new Vector3(offset.x / bounds.extents.x, offset.y / bounds.extents.y, offset.z / bounds.extents.z);
        Vector3 pivot = Vector3.Scale(bounds.extents, extent);
        Vector3 deltaPos = Vector3.Scale(pivot, meshObject.transform.localScale);
        meshObject.transform.position -= deltaPos;
        Vector3[] verts = mesh.vertices;
        for(int i = 0; i < verts.Length; i++) {
            verts[i] += pivot;
            //Debug.Log(verts[i].y);
            //if(i >= groundVertices.Length)
            //    topVertices[i - groundVertices.Length] = verts[i];
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();
        for(int i = 0; i < children.Length; i++)
            children[i].transform.parent = meshObject.transform;

     //   Debug.Log("Dat deltaPos.x: " + deltaPos.x);
     //   Debug.Log("Dat deltaPos.y: " + deltaPos.y);
     //   Debug.Log("Dat deltaPos.z: " + deltaPos.z);

        return deltaPos;
    }

    public static void scale(State state, Vector3 xyz) {
        //state.getObject().GetComponent<MeshFilter>().mesh.RecalculateBounds();
        Vector3 pivotOffset = centerPivot(state.getObject());
        //Debug.Log("getTop[0].x PRIOR to scale: " + state.getTop()[0].x);
        state.getObject().transform.localScale = Vector3.Scale(state.getObject().transform.localScale, xyz);
        Debug.Log("PivotOffset: " + pivotOffset);
        state.setPivotOffset(pivotOffset);
        //Debug.Log("getTop[0].x AFTER scaling: " + state.getTop()[0].x);
    //    state.getObject().transform.position += pivotOffset;
        //Debug.Log("Dat pivotOffset.x: " + pivotOffset.x);
        //Debug.Log("Dat pivotOffset.y: " + pivotOffset.y);
        //Debug.Log("Dat pivotOffset.z: " + pivotOffset.z);
       // state.getObject().transform.position += Vector3.Scale(pivotOffset, state.getObject().transform.localScale);
        //state.getObject().transform.position += Vector3.Scale(pivotOffset, xyz);
        //Debug.Log("WTFT!");
        
        //state.getObject().transform.Translate(
     /*   Vector3[] face = state.getFace();
        Debug.Log("SCALE-Y: " + face[0].y);
        for(int i = 0; i < face.Length; i++) {*/
            /*face[i].x *= x; */
       
            /*face[i].y *= y; */
        
            /*face[i].z *= z; */ 
    /*    face[i].x -= face[i].x * (1 - x) * 0.5f;
        face[i].y -= face[i].y * (1 - y) * 0.5f;
        face[i].z -= face[i].z * (1 - z) * 0.5f;
        }
        state.setFace(face);*/
    }

    public static void translate(State state, Vector3 xyz) {
        state.getObject().transform.Translate(xyz);
        //state.getObject().transform.position += xyz;
  /*      Vector3[] face = state.getFace();
        for(int i = 0; i < face.Length; i++) {
            face[i].x += x;
            face[i].y += y;
            face[i].z += z;
        }
        state.setFace(face);*/
    }
}

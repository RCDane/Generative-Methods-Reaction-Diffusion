using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GeometryUtilities 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public static Mesh CombineVertices(Mesh mesh, float threshold)
    {
        Vector3[] positions = mesh.vertices;
        Vector4[] tangents = mesh.tangents;
        int[] vertexIndices = mesh.GetIndices(0);
        List<Vector3> newVertices = new List<Vector3>(positions.Length);
        List<Vector4> newTangents = new List<Vector4>(positions.Length);
        Dictionary<int, int> indexMap = new Dictionary<int, int>(vertexIndices.Length);
        float sqrThreshold = threshold * threshold;

        for (int i = 0; i < positions.Length; i++)
        {
            if (indexMap.ContainsKey(i)) continue;  // This vertex has already been merged

            Vector3 currentVertex = positions[i];
            Vector3 currentTangent = tangents[i];
            int newIndex = newVertices.Count;
            newVertices.Add(currentVertex);
            newTangents.Add(currentTangent);
            indexMap[i] = newIndex;

            for (int j = i + 1; j < positions.Length; j++)
            {
                if (Vector3.SqrMagnitude(currentVertex - positions[j]) < sqrThreshold)
                {
                    indexMap[j] = newIndex;  // Map all close vertices to the new index
                }
            }
        }

        int[] newVertexIndices = new int[vertexIndices.Length];
        for (int i = 0; i < vertexIndices.Length; i++)
        {
            newVertexIndices[i] = indexMap[vertexIndices[i]];
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.tangents = newTangents.ToArray();
        newMesh.SetIndices(newVertexIndices, mesh.GetTopology(0), 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        // newMesh.RecalculateTangents();
        return newMesh;
    }

    public static Mesh WeldVertices(Mesh mesh, float threshold = 0.01f)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        Dictionary<Vector3, int> set = new Dictionary<Vector3, int>();
        List<int> indices = new List<int>();
        int[] map = new int[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            if (!set.ContainsKey(vertices[i]))
            {
                set.Add(vertices[i], indices.Count);
                map[i] = indices.Count;
                indices.Add(i);
            }
            else
            {
                map[i] = set[vertices[i]];
            }
        }

        Vector3[] newVertices = new Vector3[indices.Count];
        Vector3[] newNormals = new Vector3[indices.Count];
        Vector4[] newTangents = new Vector4[indices.Count];
        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            newVertices[i] = vertices[index];
            newNormals[i] = normals[index];
            newTangents[i] = tangents[index];
        }
        int[] newTriangles = mesh.triangles;
        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] = map[newTriangles[i]];
        }
        mesh.triangles = newTriangles;
        mesh.vertices = newVertices;
        mesh.normals = newNormals;
        mesh.tangents = newTangents;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    // public static List<int>[]  FindNeighbors(Mesh mesh, out int maxValence){
    //     int[] faces = mesh.triangles;
    //     int vertexCount = mesh.vertexCount;
    //     List<int>[] neighborsList = new List<int>[vertexCount];
    //     maxValence = 0;
    //     for (int i = 0; i < vertexCount; i++){
    //         neighborsList[i] = new List<int>(maxValence); // by using the current maxValence, we can avoid unnecessary allocations
    //         for (int j = 0; j < faces.Length; j += 3){ // triangle faces come in groups of 3
    //             int v1 = faces[j];
    //             int v2 = faces[j+1];
    //             int v3 = faces[j+2];
    //             if (v1 == i || v2 == i | v3 == i){ // one of the vertices in the triangle has to be vertex i
    //                 if (v1 != i && !neighborsList[i].Contains(v1)) {
    //                     neighborsList[i].Add(v1);
    //                 }
    //                 if (v2 != i && !neighborsList[i].Contains(v2)) {
    //                     neighborsList[i].Add(v2);
    //                 }
    //                 if (v3 != i && !neighborsList[i].Contains(v3)) {
    //                     neighborsList[i].Add(v3);
    //                 }
    //             }
    //             // We store the current highest valence we have found
    //             maxValence = math.max(neighborsList[i].Count, maxValence);

    //         }
    //     }
        
    //     return neighborsList;
    // }

    public static (int[][], int[]) FindNeighbors(Mesh mesh, out int maxValence)
    {
        int vertexCount = mesh.vertexCount;
        int[] triangles = mesh.triangles;
        int[][] neighborsList = new int[vertexCount][]; //Enumerable.Range(0, vertexCount).Select(_ => new List<int>()).ToArray();
        int[] valence = new int[vertexCount];
        List<int3>[] faces = Enumerable.Range(0, vertexCount).Select(_ => new List<int3>()).ToArray();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i+1];
            int v3 = triangles[i+2];
            faces[v1].Add(new int3(v1, v2, v3));
            faces[v2].Add(new int3(v2, v3, v1));
            faces[v3].Add(new int3(v3, v1, v2));
        }

        maxValence = 0;
        for (int i = 0; i < vertexCount; i++){
            List<int3> nfaces = faces[i];
            neighborsList[i] = new int[nfaces.Count];
            int curr = 0;
            for (int j = 0; j < nfaces.Count; j++)
            {
                int3 face = nfaces[curr];
                neighborsList[i][j] = face[1];
                int k = 0;
                for (; k < nfaces.Count; k++)
                {
                    if (curr == k) continue;
                    if (face[1] == nfaces[k][2]) break;
                }
                if (k == curr || k >= nfaces.Count)
                {
                    Debug.Log($"{k}, {curr}, {nfaces.Count}");
                    Debug.Log($"{face[0]} {face[1]} {face[2]}");
                    nfaces.ForEach(x => Debug.Log(x));
                    throw new Exception("OOPS");
                }
                curr = k;
            }
            valence[i] = neighborsList[i].Length;
            maxValence = math.max(neighborsList[i].Length, maxValence);
        }
        
        return (neighborsList, valence);
    }

    public static List<int3>[] FindNeighbors2(Mesh mesh, out int maxValence)
    {
        int vertexCount = mesh.vertexCount;
        int[] triangles = mesh.triangles;
        List<int3>[] neighborsList = new List<int3>[vertexCount]; //Enumerable.Range(0, vertexCount).Select(_ => new List<int>()).ToArray();
        int[] valence = new int[vertexCount];
        List<int3>[] faces = Enumerable.Range(0, vertexCount).Select(_ => new List<int3>()).ToArray();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i+1];
            int v3 = triangles[i+2];
            faces[v1].Add(new int3(v1, v2, v3));
            faces[v2].Add(new int3(v2, v3, v1));
            faces[v3].Add(new int3(v3, v1, v2));
        }

        maxValence = 0;
        for (int i = 0; i < vertexCount; i++){
            List<int3> nfaces = faces[i];
            neighborsList[i] = new List<int3>(nfaces.Count);
            int curr = 0;
            for (int j = 0; j < nfaces.Count; j++)
            {
                int3 face = nfaces[curr];
                // neighborsList[i].Add(face[1]);
                int a = face[1];
                int b = face[2];
                int k = 0;
                for (; k < nfaces.Count; k++)
                {
                    if (curr == k) continue;
                    if (face[1] == nfaces[k][2]) break;
                }
                if (k == curr || k >= nfaces.Count)
                {
                    
                    Debug.Log($"{k}, {curr}, {nfaces.Count}");
                    Debug.Log($"{face[0]} {face[1]} {face[2]}");
                    nfaces.ForEach(x => Debug.Log(x));
                    throw new Exception("OOPS");
                }
                int c = nfaces[k][1];
                neighborsList[i].Add(new int3(a,b,c));
                curr = k;
            }
            valence[i] = neighborsList[i].Count;
            maxValence = math.max(neighborsList[i].Count, maxValence);
        }
        
        return neighborsList;
    }

}

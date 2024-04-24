using System;
using System.Collections.Generic;
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
        int[] vertexIndices = mesh.GetIndices(0);
        List<Vector3> newVertices = new List<Vector3>();
        Dictionary<int, int> indexMap = new Dictionary<int, int>();
        float sqrThreshold = threshold * threshold;

        for (int i = 0; i < positions.Length; i++)
        {
            if (indexMap.ContainsKey(i)) continue;  // This vertex has already been merged

            Vector3 currentVertex = positions[i];
            int newIndex = newVertices.Count;
            newVertices.Add(currentVertex);
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
        newMesh.SetIndices(newVertexIndices, mesh.GetTopology(0), 0);
        newMesh.RecalculateBounds();
        return newMesh;
    }

    public static List<int>[]  FindNeighbors(Mesh mesh, out int maxValence){
        int[] faces = mesh.triangles;
        int vertexCount = mesh.vertexCount;
        List<int>[] neighborsList = new List<int>[vertexCount];
        maxValence = 0;
        for (int i = 0; i < vertexCount; i++){
            neighborsList[i] = new List<int>(maxValence); // by using the current maxValence, we can avoid unnecessary allocations
            for (int j = 0; j < faces.Length; j += 3){ // triangle faces come in groups of 3
                int v1 = faces[j];
                int v2 = faces[j+1];
                int v3 = faces[j+2];
                if (v1 == i || v2 == i | v3 == i){ // one of the vertices in the triangle has to be vertex i
                    if (v1 != i && !neighborsList[i].Contains(v1)) {
                        neighborsList[i].Add(v1);
                    }
                    if (v2 != i && !neighborsList[i].Contains(v2)) {
                        neighborsList[i].Add(v2);
                    }
                    if (v3 != i && !neighborsList[i].Contains(v3)) {
                        neighborsList[i].Add(v3);
                    }
                }
                // We store the current highest valence we have found
                maxValence = math.max(neighborsList[i].Count, maxValence);
                 
            }
        }
        
        return neighborsList;
    }
}

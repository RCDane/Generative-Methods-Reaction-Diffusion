using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

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


}

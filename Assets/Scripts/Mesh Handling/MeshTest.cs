using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshFilter filter =GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        Mesh newMesh = GeometryUtilities.CombineVertices(mesh, 0.001f);       
        int valence = 0;
        List<int>[] neighbors = GeometryUtilities.FindNeighbors(newMesh, out valence);
        print("valence" + valence);
        filter.mesh = newMesh;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

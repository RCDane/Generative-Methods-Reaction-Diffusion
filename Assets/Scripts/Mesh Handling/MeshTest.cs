using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    private Material valenceColoringMaterial;

    Mesh _mesh;

    List<int>[] _neighbors;

    ComputeBuffer _ColorBuffer;
    ComputeBuffer _NeighborBuffer;

    int _valence;
    int _kernel; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        valenceColoringMaterial = renderer.material;
        MeshFilter filter =GetComponent<MeshFilter>();
        _mesh = filter.mesh;
        Mesh newMesh = GeometryUtilities.CombineVertices(_mesh, 0.001f);      
        int vertexCount = newMesh.vertexCount;

        int[] _;
        (_neighbors, _) = GeometryUtilities.FindNeighbors(newMesh, out _valence);


        _kernel = computeShader.FindKernel("CSValenceColoring");

        computeShader.SetInt("_maxValancy", _valence);
        computeShader.SetInt("_vertexCount", vertexCount);

        PrepareBuffers();
        

        computeShader.Dispatch(_kernel, vertexCount, 1, 1);
        
        float3[] color = new float3[vertexCount];
        _ColorBuffer.GetData(color);
        for (int i = 0; i < vertexCount; i++)
        {
            if (!color[i].Equals(new float3(1,1,1)))
                Debug.Log(color[i]);
        }


        print("valence" + _valence);
        filter.mesh = newMesh;

    }

    ComputeBuffer WriteNeighborData(){
        int stride = sizeof(int);
        _NeighborBuffer = new ComputeBuffer(_valence*_neighbors.Length, stride, 
                                    ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);
        var buffer = _NeighborBuffer.BeginWrite<int>(0, _valence*_neighbors.Length);
        for (int i = 0; i < _neighbors.Length; i++)
        {
            int currentValence = _neighbors[i].Count;
            for (int j = 0; j < _valence; j++){
                
                int index = i*_valence+j;
                buffer[index] = j < currentValence ? _neighbors[i][j] : -1;
            }
        }
        _NeighborBuffer.EndWrite<int>(_valence*_neighbors.Length);
        return _NeighborBuffer;
    }

    void PrepareBuffers(){
        var neighborBuffPosition = Shader.PropertyToID("_neighboors");
        var vertexColorBuffPosition = Shader.PropertyToID("_color");

        _ColorBuffer = 
                new ComputeBuffer(_mesh.vertexCount, sizeof(float) * 3);


        computeShader.SetBuffer(_kernel, "_color", _ColorBuffer);
        
        valenceColoringMaterial.SetBuffer("_ColorBuffer", _ColorBuffer );

        ComputeBuffer neighborBuffer = WriteNeighborData();

        computeShader.SetBuffer(_kernel, neighborBuffPosition, neighborBuffer);
    }


}

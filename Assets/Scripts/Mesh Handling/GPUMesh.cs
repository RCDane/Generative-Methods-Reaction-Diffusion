using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GPUMesh : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    public ComputeShader computeShader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
    }

    int CalculateKernelSize(int ThreadGroupSize, int threadCount)
    {
        
        int dispatchedThreadGroupSize = threadCount / ThreadGroupSize;

        if (dispatchedThreadGroupSize % ThreadGroupSize == 0) return dispatchedThreadGroupSize;
   
        while (dispatchedThreadGroupSize % ThreadGroupSize != 0)
        {
            dispatchedThreadGroupSize += 1;
            if (dispatchedThreadGroupSize % ThreadGroupSize != 0) continue;
       
       
            Debug.LogFormat("Initial threads: {0}", ThreadGroupSize);
            Debug.LogFormat("Threads X used: {0}", dispatchedThreadGroupSize);
            return dispatchedThreadGroupSize;
        }
        return dispatchedThreadGroupSize;
    }

    void PrepareBuffers()
    {
        
    }
    
    
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            var kernel = computeShader.FindKernel("CSMain");

            uint sizeX = 0;
            computeShader.GetKernelThreadGroupSizes(kernel, out sizeX, out _, out _);
            
            
            
            Vector3[] vertices = mesh.vertices;
            var dispatchAmount = CalculateKernelSize((int)sizeX, vertices.Length);
            

            GraphicsBuffer vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, vertices.Length, 12);
            vertexBuffer.SetData(vertices);
            
            var inputMeshPosition = Shader.PropertyToID("_inputMesh");
            var outputMeshPosition = Shader.PropertyToID("_outputMesh");
            
            
            GraphicsBuffer outputvertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex,vertices.Length, 12);

            computeShader.SetBuffer(kernel, inputMeshPosition, vertexBuffer);
            computeShader.SetBuffer(kernel, outputMeshPosition, outputvertexBuffer);
            computeShader.Dispatch(kernel, dispatchAmount, 1, 1);
            

            outputvertexBuffer.GetData(vertices);
            mesh.SetVertices(vertices);
            vertexBuffer.Release();
            outputvertexBuffer.Release();
        }
    }

    void printVertices(Vector3[] vertices){
        foreach (var vertex in vertices)
        {
            Debug.Log(vertex);
        }
        Console.WriteLine("\n");
    }
    
}

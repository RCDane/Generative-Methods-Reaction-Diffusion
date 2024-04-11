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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            var kernel = computeShader.FindKernel("CSMain");
            Vector3[] vertices = mesh.vertices;


            GraphicsBuffer vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, vertices.Length*sizeof(float)*3, 1);
            vertexBuffer.SetData(vertices,0, 0 , vertices.Length);
            
            var inputMeshPosition = Shader.PropertyToID("inputMesh");
            var outputMeshPosition = Shader.PropertyToID("outputMesh");
            
            
            GraphicsBuffer outputvertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, vertices.Length*sizeof(float)*3, 1);
            print(vertices.Length);

            computeShader.SetBuffer(kernel, inputMeshPosition, vertexBuffer);
            computeShader.SetBuffer(kernel, outputMeshPosition, outputvertexBuffer);
            computeShader.Dispatch(kernel, vertices.Length*3*sizeof(float), 1, 1);
            

            outputvertexBuffer.GetData(vertices);
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
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

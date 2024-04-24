using System;
using System.Collections.Generic;
using UnityEngine;

public class GPULaplacian : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    public ComputeShader computeShader;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GeometryUtilities.CombineVertices(meshFilter.mesh, 0.0001f);
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
       
       
            // Debug.LogFormat("Initial threads: {0}", ThreadGroupSize);
            // Debug.LogFormat("Threads X used: {0}", dispatchedThreadGroupSize);
            return dispatchedThreadGroupSize;
        }
        return dispatchedThreadGroupSize;
    }

    void Update()
    {
        // if(Input.GetMouseButtonDown(0))
        {
            var kernel = computeShader.FindKernel("CSLaplacian");

            uint sizeX = 0;
            computeShader.GetKernelThreadGroupSizes(kernel, out sizeX, out _, out _);
            
            Vector3[] vertices = mesh.vertices;
            var dispatchAmount = CalculateKernelSize((int)sizeX, vertices.Length);
            
            int inputMeshPosition = Shader.PropertyToID("_inputMesh");
            GraphicsBuffer vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, vertices.Length, 12);
            vertexBuffer.SetData(vertices);
            computeShader.SetBuffer(kernel, inputMeshPosition, vertexBuffer);

            int outputMeshPosition = Shader.PropertyToID("_outputMesh");
            GraphicsBuffer outputvertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex,vertices.Length, 12);
            computeShader.SetBuffer(kernel, outputMeshPosition, outputvertexBuffer);


            (List<int>[] neighbors, int[] valence) = GeometryUtilities.FindNeighbors(mesh, out int maxValence);

            // var test = GeometryUtilities.FindNeighbors2(mesh, out int _);

            // test[0].ForEach(x => Debug.Log(x));

            int[] vertexNeighbors = new int[mesh.vertexCount*maxValence];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                List<int> neigh = neighbors[i];
                for (int j = 0; j < maxValence; j++)
                {
                    if (j < neigh.Count)
                    {
                        vertexNeighbors[i*maxValence+j] = neigh[j];
                    }
                    else
                    {
                        vertexNeighbors[i*maxValence+j] = -1;
                    }
                }
            }
            int inputMeshVertexNeighbours = Shader.PropertyToID("_inputVertexNeighbours");
            ComputeBuffer neighborBuffer = new ComputeBuffer(vertices.Length*maxValence, sizeof(int));
            neighborBuffer.SetData(vertexNeighbors);
            computeShader.SetBuffer(kernel, inputMeshVertexNeighbours, neighborBuffer);

            int inputMaxValence = Shader.PropertyToID("_inputMaxValence");
            computeShader.SetInt(inputMaxValence, maxValence);

            int inputValence = Shader.PropertyToID("_inputValence");
            ComputeBuffer valenceBuffer = new ComputeBuffer(vertices.Length, sizeof(int));
            valenceBuffer.SetData(valence);
            computeShader.SetBuffer(kernel, inputValence, valenceBuffer);

            computeShader.Dispatch(kernel, dispatchAmount, 1, 1);
            
            outputvertexBuffer.GetData(vertices);
            mesh.SetVertices(vertices);
            vertexBuffer.Release();
            outputvertexBuffer.Release();
        }
    }
}

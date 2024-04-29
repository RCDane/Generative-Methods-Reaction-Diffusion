using UnityEngine;
using System.Collections.Generic;
using System;
public class GPUWaveEquation : MonoBehaviour
{
    
    MeshFilter meshFilter;
    Mesh mesh;
    public ComputeShader computeShader;
    public Material WaveMat;
    private GraphicsBuffer _vertexBuffer;
    private ComputeBuffer _heightBuffer;
    private ComputeBuffer _verticalSpeedBuffer;
    private ComputeBuffer _neighborBuffer;
    private ComputeBuffer _valenceBuffer;
    private Vector3[] _vertices;
    private int[] _vertexNeighbors;
    private int[] _valence;
    int _dispatchSize = 1;
    int _kernel = -1;
    int _maxValency;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GeometryUtilities.CombineVertices(meshFilter.mesh, 0.0001f);
        mesh = meshFilter.mesh;
        WaveMat = GetComponent<Renderer>().material;
        Setup();

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

    void Setup()
    {
        uint sizeX = 0;
        _kernel = computeShader.FindKernel("CSWaveEquation");
        computeShader.GetKernelThreadGroupSizes(_kernel, out sizeX, out _, out _);

        _vertices = mesh.vertices;
        _dispatchSize = CalculateKernelSize((int)sizeX, _vertices.Length);

        List<int>[] neighbors;
        (neighbors, _valence) = GeometryUtilities.FindNeighbors(mesh, out _maxValency);

        _vertexNeighbors = new int[mesh.vertexCount*_maxValency];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            List<int> neigh = neighbors[i];
            for (int j = 0; j < _maxValency; j++)
            {
                if (j < neigh.Count)
                {
                    _vertexNeighbors[i*_maxValency+j] = neigh[j];
                }
                else
                {
                    _vertexNeighbors[i*_maxValency+j] = -1;
                }
            }
        }
        SetupBuffers();
    }



    ComputeBuffer CreateEmptyBuffer<T>(int size, int typeSize)
    {
        ComputeBuffer buffer = new ComputeBuffer(size, typeSize);
        
        buffer.SetData(new T[size]);
        return buffer;
    }

    void RandomInput(ComputeBuffer buffer, int amount){
        for (int i = 0; i < amount; i++)
        {
            float val = UnityEngine.Random.Range(0.0f, 1.0f);
            int index = UnityEngine.Random.Range(0, buffer.count);
            buffer.SetData(new float[]{val}, 0, (int)index, 1);
        }
    }

    void SetupBuffers()
    {
        int inputMeshPosition = Shader.PropertyToID("_inputMesh");
        _vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, _vertices.Length, 3*sizeof(float));
        _vertexBuffer.SetData(_vertices);
        computeShader.SetBuffer(_kernel, inputMeshPosition, _vertexBuffer);

        
        
        int inputMeshVertexNeighbours = Shader.PropertyToID("_inputVertexNeighbours");
        _neighborBuffer = new ComputeBuffer(_vertices.Length*_maxValency, sizeof(int));
        _neighborBuffer.SetData(_vertexNeighbors);
        computeShader.SetBuffer(_kernel, inputMeshVertexNeighbours, _neighborBuffer);

        int inputMaxValence = Shader.PropertyToID("_inputMaxValence");
        computeShader.SetInt(inputMaxValence, _maxValency);

        int inputValence = Shader.PropertyToID("_inputValence");
        _valenceBuffer = new ComputeBuffer(_vertices.Length, sizeof(int));
        _valenceBuffer.SetData(_valence);
        computeShader.SetBuffer(_kernel, inputValence, _valenceBuffer);

        int inputHeight = Shader.PropertyToID("_waveHeight");
        _heightBuffer = CreateEmptyBuffer<float>(_vertices.Length, sizeof(float));
        RandomInput(_heightBuffer, 10);

        computeShader.SetBuffer(_kernel, inputHeight, _heightBuffer);

        int inputVerticalSpeed = Shader.PropertyToID("_waveVerticalSpeed");
        _verticalSpeedBuffer = CreateEmptyBuffer<float>(_vertices.Length, sizeof(float));
        computeShader.SetBuffer(_kernel, inputVerticalSpeed, _verticalSpeedBuffer);

        
        WaveMat.SetBuffer("_waveHeight", _heightBuffer);
        WaveMat.SetBuffer("_waveVerticalSpeed", _verticalSpeedBuffer);



        _copyBuffer = new Vector3[_vertices.Length];
    }

    ~GPUWaveEquation(){
        _neighborBuffer.Release();
        _valenceBuffer.Release();   
        _vertexBuffer.Release();
    }


    Vector3[] _copyBuffer;

    void Update()
    {
        // if(Input.GetMouseButtonDown(0))
        {

            computeShader.Dispatch(_kernel, _dispatchSize, 1, 1);

            // _outputColorBuffer.GetData(_color);

            // mesh.RecalculateNormals();
        }
    }
}

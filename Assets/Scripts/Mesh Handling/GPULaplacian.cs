using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class GPULaplacian : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    public ComputeShader computeShader;
    public Material GPUColorMat;
    private GraphicsBuffer _vertexBuffer;
    private ComputeBuffer _outputColorBuffer;
    private ComputeBuffer _inputColorBuffer;
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
        Vector3[] vertices = mesh.vertices;

        // TODO: temp uv fix
        float maxX = mesh.vertices.Max(x => x.x);
        float maxY = mesh.vertices.Max(x => x.y);
        float maxZ = mesh.vertices.Max(x => x.z);
        Vector2[] uv = new Vector2[mesh.vertices.Length];
        int vertexCount = mesh.vertices.Length;
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 v = vertices[i];
            uv[i] = new Vector2(v.x / maxX, v.y*v.z / (maxY*maxZ));
        }
        mesh.uv = uv;

        GPUColorMat = GetComponent<Renderer>().material;
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
        _kernel = computeShader.FindKernel("CSLaplacian");
        computeShader.GetKernelThreadGroupSizes(_kernel, out sizeX, out _, out _);

        _vertices = mesh.vertices;
        _dispatchSize = CalculateKernelSize((int)sizeX, _vertices.Length);

        int[][] neighbors;
        (neighbors, _valence) = GeometryUtilities.FindNeighbors(mesh, out _maxValency);

        _vertexNeighbors = new int[mesh.vertexCount*_maxValency];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            int[] neigh = neighbors[i];
            for (int j = 0; j < _maxValency; j++)
            {
                if (j < neigh.Length)
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

    void SetupBuffers()
    {
        int inputMeshPosition = Shader.PropertyToID("_inputMesh");
        _vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, _vertices.Length, 3*sizeof(float));
        _vertexBuffer.SetData(_vertices);
        computeShader.SetBuffer(_kernel, inputMeshPosition, _vertexBuffer);

        int inputColor = Shader.PropertyToID("_inputColors");
        _inputColorBuffer = new ComputeBuffer(_vertices.Length, 3*sizeof(float));
        _inputColorBuffer.SetData(Enumerable.Range(0, _vertices.Length).Select(_ => new Vector3(0.0f, 0.0f, 1.0f)).ToArray());
        computeShader.SetBuffer(_kernel, inputColor, _inputColorBuffer);

        int outputColor = Shader.PropertyToID("_outputColors");
        _outputColorBuffer = new ComputeBuffer(_vertices.Length, 3*sizeof(float));
        computeShader.SetBuffer(_kernel, outputColor, _outputColorBuffer);
        
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

        GPUColorMat.SetBuffer("_ColorBuffer", _outputColorBuffer);

        _copyBuffer = new Vector3[_vertices.Length];
    }

    ~GPULaplacian(){
        _outputColorBuffer.Release();
        _inputColorBuffer.Release();
        _neighborBuffer.Release();
        _valenceBuffer.Release();   
        _vertexBuffer.Release();
    }


    Vector3[] _copyBuffer;

	Ray ray;
	RaycastHit hit;

    void Update()
    {
        // if(Input.GetMouseButtonDown(0))
        {
            computeShader.Dispatch(_kernel, _dispatchSize, 1, 1);
            _outputColorBuffer.GetData(_copyBuffer);

            bool leftMouse = Input.GetMouseButtonDown(0);
            bool rightMouse = Input.GetMouseButtonDown(1);
            if (leftMouse || rightMouse)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 v = leftMouse ? new Vector3(0.0f, 1.0f, 0.0f) : new Vector3(1.0f, 0.0f, 0.0f);
                    Debug.Log(hit.triangleIndex);
                    _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex]);
                    _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex+1]);
                    _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex+2]);
                }
            }
            _inputColorBuffer.SetData(_copyBuffer);

            // _outputColorBuffer.GetData(_color);

            // mesh.RecalculateNormals();
        }
    }
}

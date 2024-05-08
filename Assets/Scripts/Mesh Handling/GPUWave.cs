using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.VFX;
using Unity.Collections;
using Unity.Mathematics;
public class GPUWave : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    public ComputeShader computeShader;
    public Material GPUColorMat;
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _normalBuffer;
    private GraphicsBuffer _inputColorBuffer;
    private ComputeBuffer _neighborBuffer;
    private ComputeBuffer _valenceBuffer;
    private VisualEffect _visualEffect;
    private Vector3[] _vertices;
    private int[] _vertexNeighbors;
    private int[] _valence;
    int _dispatchSize = 1;
    int _kernel = -1;
    int _maxValency;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GeometryUtilities.CombineVertices(meshFilter.mesh, 0.00001f);
        mesh = meshFilter.mesh;
        meshFilter.sharedMesh = mesh;

        GPUColorMat = GetComponent<Renderer>().material;


        if (TryGetComponent<VisualEffect>(out _visualEffect))
        {
            _visualEffect.SetMesh("Mesh", mesh);
        }
        
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

        _inputColorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, _vertices.Length, 3*sizeof(float));
        // float3[] tmpColor = new float3[_vertices.Length];
        // _inputColorBuffer.GetData(tmpColor);
        // for (int i = 0; i < _vertices.Length; i++)
        // {
        //     tmpColor[i] = new float3(0.0f, 0.0f, 0.0f);
        // }


        
        
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

        GPUColorMat.SetBuffer("_ColorBuffer", _inputColorBuffer);

        _normalBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, _vertices.Length, 3*sizeof(float));
        _normalBuffer.SetData(mesh.normals);


        _copyBuffer = new Vector3[_vertices.Length];
        // ensure that input and output is empty
        for (int i = 0; i < _vertices.Length; i++)
        {
            _copyBuffer[i] = new Vector3(0.0f, 0.0f, 0.0f);
        }
        _inputColorBuffer.SetData(_copyBuffer);
        computeShader.SetBuffer(_kernel, inputColor, _inputColorBuffer);

    }

    ~GPUWave(){
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
        computeShader.Dispatch(_kernel, _dispatchSize, 1, 1);

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _inputColorBuffer.GetData(_copyBuffer);

            bool leftMouse = Input.GetMouseButtonDown(0);
            bool rightMouse = Input.GetMouseButtonDown(1);
            if (leftMouse || rightMouse)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.triangleIndex);
                    // if (hit.triangleIndex >= 0)
                    {
                        Vector3 v = leftMouse ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(0.0f, 1.0f, 0.0f);
                        int faceIndex = 3*hit.triangleIndex;
                        int vertexIndex = mesh.triangles[faceIndex];
                        _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex]);
                        _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex+1]);
                        _copyBuffer.SetValue(v, mesh.triangles[3*hit.triangleIndex+2]);
                    }
                }
            }
            _inputColorBuffer.SetData(_copyBuffer);

            // _outputColorBuffer.GetData(_color);

            // mesh.RecalculateNormals();
        }
    }
}

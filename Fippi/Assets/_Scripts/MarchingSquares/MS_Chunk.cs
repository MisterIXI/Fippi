using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MS_Chunk : MonoBehaviour
{
    [SerializeField] private bool _showGizmos = false;
    public Vector3 Offset { get; private set; }
    [field: SerializeField] public Vector2Int ID { get; private set; } = Vector2Int.one * -1;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private ChunkSettings _chunkSettings;
    private int _tileCount;
    private int _tileCountPlusOne => _tileCount + 1;
    private float _unitSize;
    private List<int>[] _meshTriangles;

    private void Start()
    {
    }

    public void InitMesh(ChunkSettings chunkSettings, Vector2Int id)
    {
        // init local values
        ID = id;
        _chunkSettings = chunkSettings;
        _unitSize = chunkSettings.UnitSize;
        _tileCount = chunkSettings.TilesPerAxis;
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshTriangles = new List<int>[chunkSettings.WallMaterials.Length];
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;
        Vector3[] vertices = new Vector3[_tileCount * _tileCount * 8];
        Vector2[] _uv = new Vector2[_tileCount * _tileCount * 8];
        Offset = -Vector2.one * (_tileCount * _unitSize) / 2 - (Vector2.one * 0.5f);
        float uvSteps = 1f / _tileCount;
        for (int y = 0; y < _tileCount; y++)
        {
            for (int x = 0; x < _tileCount; x++)
            {
                int arrIndex = (x + y * _tileCount) * 8;
                vertices[arrIndex + 0] = new Vector3(x + Offset.x, y + Offset.y, 0);
                vertices[arrIndex + 1] = new Vector3(x + Offset.x + 0.5f, y + Offset.y, 0);
                vertices[arrIndex + 2] = new Vector3(x + Offset.x + 1, y + Offset.y, 0);
                vertices[arrIndex + 3] = new Vector3(x + Offset.x, y + Offset.y + 0.5f, 0);
                vertices[arrIndex + 4] = new Vector3(x + Offset.x + 1, y + Offset.y + 0.5f, 0);
                vertices[arrIndex + 5] = new Vector3(x + Offset.x, y + Offset.y + 1, 0);
                vertices[arrIndex + 6] = new Vector3(x + Offset.x + 0.5f, y + Offset.y + 1, 0);
                vertices[arrIndex + 7] = new Vector3(x + Offset.x + 1, y + Offset.y + 1, 0);

                _uv[arrIndex + 0] = new Vector2(0.0f, 0.0f);
                _uv[arrIndex + 1] = new Vector2(0.5f, 0.0f);
                _uv[arrIndex + 2] = new Vector2(1.0f, 0.0f);
                _uv[arrIndex + 3] = new Vector2(0.0f, 0.5f);
                _uv[arrIndex + 4] = new Vector2(1.0f, 0.5f);
                _uv[arrIndex + 5] = new Vector2(0.0f, 1.0f);
                _uv[arrIndex + 6] = new Vector2(0.5f, 1.0f);
                _uv[arrIndex + 7] = new Vector2(1.0f, 1.0f);

                // Vector2 uvOffset = new Vector2(x, y) / _tileCount;
                // _uv[arrIndex + 0] = new Vector2(uvOffset.x + (0.0f * uvSteps), uvOffset.y + (0.0f * uvSteps));
                // _uv[arrIndex + 1] = new Vector2(uvOffset.x + (0.5f * uvSteps), uvOffset.y + (0.0f * uvSteps));
                // _uv[arrIndex + 2] = new Vector2(uvOffset.x + (1.0f * uvSteps), uvOffset.y + (0.0f * uvSteps));
                // _uv[arrIndex + 3] = new Vector2(uvOffset.x + (0.0f * uvSteps), uvOffset.y + (0.5f * uvSteps));
                // _uv[arrIndex + 4] = new Vector2(uvOffset.x + (1.0f * uvSteps), uvOffset.y + (0.5f * uvSteps));
                // _uv[arrIndex + 5] = new Vector2(uvOffset.x + (0.0f * uvSteps), uvOffset.y + (1.0f * uvSteps));
                // _uv[arrIndex + 6] = new Vector2(uvOffset.x + (0.5f * uvSteps), uvOffset.y + (1.0f * uvSteps));
                // _uv[arrIndex + 7] = new Vector2(uvOffset.x + (1.0f * uvSteps), uvOffset.y + (1.0f * uvSteps));
            }
        }
        _mesh.vertices = vertices;
        _meshRenderer.materials = chunkSettings.WallMaterials.Clone() as Material[];
        _mesh.uv = _uv;
        _mesh.subMeshCount = _meshRenderer.materials.GetLength(0);
        // // generate UV
        // Vector2[] uvs = new Vector2[_tileCount * _tileCount * 8];
        // for (int i = 0; i < uvs.Length; i++)
        // {
        //     uvs[i] = new Vector2(vertices[i].x / (_tileCount * _unitSize), vertices[i].y / (_tileCount * _unitSize));
        // }
        // generate background
        GameObject groundplane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        groundplane.transform.localScale = new(_tileCount, _tileCount, 1);
        groundplane.transform.parent = transform;
        groundplane.transform.localPosition = new Vector3(-0.5f, -0.5f, 1);
        groundplane.GetComponent<MeshRenderer>().material = _chunkSettings.FloorMaterial;
    }

    public void RecalculateChunk()
    {
        int[,,] wallInfo = MarchingSquares.WallInfo;
        int lastTypeNum = (int)WallTypeExtensions.LastType;
        for (int i = 0; i < _meshTriangles.Length; i++)
        {
            _meshTriangles[i] = new List<int>();
        }
        for (int x = StartIndex.x; x < StartIndex.x + _tileCount; x++)
        {
            for (int y = StartIndex.y; y < StartIndex.y + _tileCount; y++)
            {
                // check for OOB at the right and top corner of map
                if (x + 1 >= wallInfo.GetLength(0) || y + 1 >= wallInfo.GetLength(1)) continue;

                // special check for walls
                {
                    byte points = 0;
                    if (wallInfo[x, y, 1] > 0) points |= 0b1000;
                    if (wallInfo[x + 1, y, 1] > 0) points |= 0b0100;
                    if (wallInfo[x + 1, y + 1, 1] > 0) points |= 0b0010;
                    if (wallInfo[x, y + 1, 1] > 0) points |= 0b0001;
                    int[] triangles = SquareConfigs.Triangles[points].Clone() as int[];
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        triangles[i] += (((x - StartIndex.x) + (y - StartIndex.y) * (_tileCount)) * 8);
                    }
                    _meshTriangles[0].AddRange(triangles);
                }
                // extra check for other submesh
                int offset = 1;
                for (int m = 1; m < lastTypeNum; m++)
                {
                    byte b = 0;
                    if (wallInfo[x, y, 0] == m + offset && wallInfo[x, y, 1] > 0) b |= 0b1000;
                    if (wallInfo[x + 1, y, 0] == m + offset && wallInfo[x + 1, y, 1] > 0) b |= 0b0100;
                    if (wallInfo[x + 1, y + 1, 0] == m + offset && wallInfo[x + 1, y + 1, 1] > 0) b |= 0b0010;
                    if (wallInfo[x, y + 1, 0] == m + offset && wallInfo[x, y + 1, 1] > 0) b |= 0b0001;
                    int[] triangles = SquareConfigs.Triangles[b].Clone() as int[];
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        triangles[i] += (((x - StartIndex.x) + (y - StartIndex.y) * (_tileCount)) * 8);
                    }
                    _meshTriangles[m].AddRange(triangles);
                }
            }
        }
        for (int i = 0; i < _meshTriangles.Length; i++)
        {
            // int maxV = _mesh.vertexCount;
            // // get max index with Linq
            //     int maxT = _meshTriangles[i].Max();
            // Debug.Log($"MaxV: {maxV}, MaxT: {maxT}");
            _mesh.SetTriangles(_meshTriangles[i].ToArray(), i);
        }
        _mesh.RecalculateNormals();
        //_mesh.RecalculateTangents();
    }
    #region Utility
    [SerializeField] private Vector2Int StartIndex => new Vector2Int(ID.x * _tileCount, ID.y * _tileCount);
    #endregion
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            if (MarchingSquares.WallInfo != null)
            {
                int[,,] wallInfo = MarchingSquares.WallInfo;
                float offset = -_chunkSettings.TilesPerAxis * _unitSize / 2 - 0.5f;
                Vector3 offsetVector = new Vector3(offset, offset, 0) + transform.position;
                var startIndex = StartIndex;
                // draw a sphere for each density point
                for (int y = startIndex.y; y < startIndex.y + _tileCountPlusOne; y++)
                {
                    for (int x = startIndex.x; x < startIndex.x + _tileCountPlusOne; x++)
                    {

                        Gizmos.color = wallInfo[x, y, 0] > 0 ? Color.red : Color.green;
                        Gizmos.DrawSphere(new Vector3((x - startIndex.x) * _unitSize, (y - startIndex.y) * _unitSize, -5) + offsetVector, 0.25f);
                    }
                }

            }
        }
    }
}
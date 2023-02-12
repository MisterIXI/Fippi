using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MS_Chunk_old : MonoBehaviour
{
    public int[,] Densities;
    private Mesh _mesh;
    private ChunkGenerator_old _chunkGenerator;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private ChunkSettings _chunkSettings;
    private int _tileCount;
    [SerializeField] private bool ShowGizmos = false;
    private void Start()
    {
        _chunkGenerator = transform.parent.GetComponent<ChunkGenerator_old>();
        _chunkSettings = _chunkGenerator.ChunkSettings;
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        GenerateVertices();
    }

    private void GenerateVertices()
    {
        _mesh = new Mesh();
        int tileCount = _chunkSettings.TilesPerAxis + 1;
        Densities = new int[tileCount, tileCount];
        _tileCount = _chunkSettings.TilesPerAxis;
        Vector3[] vertices = new Vector3[tileCount * tileCount * 8];
        float offset = -_chunkSettings.TilesPerAxis / 2 - 0.5f;
        for (int y = 0; y < tileCount; y++)
        {
            for (int x = 0; x < tileCount; x++)
            {

                vertices[(x + y * tileCount) * 8 + 0] = new Vector3(x + offset, y + offset, 0);
                vertices[(x + y * tileCount) * 8 + 1] = new Vector3(x + offset + 0.5f, y + offset, 0);
                vertices[(x + y * tileCount) * 8 + 2] = new Vector3(x + offset + 1, y + offset, 0);
                vertices[(x + y * tileCount) * 8 + 3] = new Vector3(x + offset, y + offset + 0.5f, 0);
                vertices[(x + y * tileCount) * 8 + 4] = new Vector3(x + offset + 1, y + offset + 0.5f, 0);
                vertices[(x + y * tileCount) * 8 + 5] = new Vector3(x + offset, y + offset + 1, 0);
                vertices[(x + y * tileCount) * 8 + 6] = new Vector3(x + offset + 0.5f, y + offset + 1, 0);
                vertices[(x + y * tileCount) * 8 + 7] = new Vector3(x + offset + 1, y + offset + 1, 0);
            }
        }
        _mesh.vertices = vertices;
        // _meshRenderer.material = _chunkSettings.WallMaterial;
        GameObject groundplane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        groundplane.transform.localScale = new(_tileCount, _tileCount, 1);
        groundplane.transform.parent = transform;
        groundplane.transform.localPosition = new Vector3(-0.5f, -0.5f, 1);
        groundplane.GetComponent<MeshRenderer>().material = _chunkSettings.FloorMaterial;
    }

    public void RecalculateChunk()
    {
        List<int[]> triangleArrays = new List<int[]>();
        for (int y = 0; y < _tileCount; y++)
        {
            for (int x = 0; x < _tileCount; x++)
            {
                byte points = 0;
                if (Densities[x, y] > 0) points |= 0b1000;
                if (Densities[x + 1, y] > 0) points |= 0b0100;
                if (Densities[x + 1, y + 1] > 0) points |= 0b0010;
                if (Densities[x, y + 1] > 0) points |= 0b0001;
                int[] triangles = SquareConfigs.Triangles[points].Clone() as int[];
                for (int i = 0; i < triangles.Length; i++)
                {
                    triangles[i] += ((x + y * (_tileCount + 1)) * 8);
                }
                triangleArrays.Add(triangles);
                // print triangles
            }
        }
        // project all arrays to one flat array
        // Debug.Log("Mesh size: " + _mesh.vertices.Length);
        _mesh.triangles = triangleArrays.SelectMany(x => x).ToArray();
        _mesh.RecalculateNormals();
        _meshFilter.mesh = _mesh;
    }


    private void OnDrawGizmos()
    {
        if (ShowGizmos)
        {
            if (Densities != null)
            {
                float offset = -_chunkSettings.TilesPerAxis / 2 - 0.5f;
                Vector3 offsetVector = new Vector3(offset, offset, 0) + transform.position;
                // draw a sphere for each density point
                for (int y = 0; y < Densities.GetLength(1); y++)
                {
                    for (int x = 0; x < Densities.GetLength(0); x++)
                    {
                        Gizmos.color = Densities[x, y] > 0 ? Color.red : Color.green;
                        Gizmos.DrawSphere(new Vector3(x, y, 0) + offsetVector, 0.1f);
                    }
                }

            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
public class ColliderUpdater : MonoBehaviour
{
    private GameObject _colliderParent;
    private PolygonCollider2D[,] _polygonColliders;
    private Vector2Int _lastPos;
    [SerializeField] private MovementSettings _movementSettings;
    private int _colliderAxisLength => _movementSettings.CollisionCheckCountPerDirection * 2 + 1;
    private float _colliderOffset => _movementSettings.CollisionCheckCountPerDirection * _unitSize;
    private float _unitSize => MarchingSquares.Instance.chunkSettings.UnitSize;
    [SerializeField] private bool _drawGizmos = true;
    private void Start()
    {
        CreateColliders();
    }
    private void Update()
    {
        Vector2Int currPos = MarchingSquares.GetIndexFromPos(transform.position);
        if (currPos != _lastPos)
        {
            UpdateColliders();
            _lastPos = currPos;
        }

    }
    private void CreateColliders()
    {
        _colliderParent = new GameObject("Colliders for: " + gameObject.name);
        _polygonColliders = new PolygonCollider2D[_colliderAxisLength, _colliderAxisLength];
        for (int x = 0; x < _colliderAxisLength; x++)
        {
            for (int y = 0; y < _colliderAxisLength; y++)
            {
                _polygonColliders[x, y] = _colliderParent.AddComponent<PolygonCollider2D>();
            }
        }
    }

    private void UpdateColliders()
    {
        Vector2Int currPos = MarchingSquares.GetIndexFromPos(transform.position);
        Vector2 position = MarchingSquares.GetPosFromIndex(currPos);
        _colliderParent.transform.position = position;
        for (int x = 0; x < _colliderAxisLength; x++)
        {
            for (int y = 0; y < _colliderAxisLength; y++)
            {
                Vector2 offset = new Vector2(-_colliderOffset, -_colliderOffset) + new Vector2(x, y) * _unitSize;
                byte squareConfig = MarchingSquares.GetWallByte((Vector2)transform.position + offset);
                var points = SquareConfigs.Edges[squareConfig].Clone() as Vector2[];
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] *= _unitSize * 0.5f;
                    points[i] += Vector2.one * _unitSize * 0.5f;
                    points[i] += offset;
                }
                // _edgeColliders[x, y].points = points;
                if (points.Length == 0)
                    _polygonColliders[x, y].pathCount = 0;
                else
                {
                    _polygonColliders[x, y].pathCount = 1;
                    _polygonColliders[x, y].points = points;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (_drawGizmos && _polygonColliders != null)
        {
            Vector3 unitOffset = _unitSize * Vector3.one * 0.5f;
            for (int x = 0; x < _colliderAxisLength; x++)
            {
                for (int y = 0; y < _colliderAxisLength; y++)
                {
                    Gizmos.color = Color.blue;
                    Vector2[] points = _polygonColliders[x, y].points;
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        Gizmos.DrawLine((Vector3)points[i] + _colliderParent.transform.position, (Vector3)points[(i + 1)] + _colliderParent.transform.position);
                    }
                }
            }
        }
    }
}
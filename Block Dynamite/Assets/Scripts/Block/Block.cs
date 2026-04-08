using System;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static event Action<Sprite, List<Vector3>> OnDropBlock;
    [SerializeField] private bool _destroyOnReturn = false;
    private bool[,] _grid = new bool[8, 8];
    private List<Vector2Int> _cellPosInShape = new List<Vector2Int>();
    private List<Cell> _cells = new List<Cell>();
    [SerializeField] private Cell _cellPrefab;
    private List<Collider2D> _cellColliders = new();
    private Sprite _cellSprite;
    private bool _dragging = false;
    [SerializeField] private Vector3 _dragOffset = new Vector3(0, 1.5f, 0);
    private Vector3 _spawnPos;
    void Update()
    {
        Vector3 mousePos = GetMouseWorldPos();

        if (Input.GetMouseButtonDown(0) && !_dragging)
        {
            foreach (var col in _cellColliders)
            {
                if (col != null && col.OverlapPoint(mousePos))
                {
                    _dragging = true;
                    transform.localScale *= 2;
                    break;
                }
            }
        }

        if (_dragging && Input.GetMouseButton(0))
        {
            transform.position = mousePos + _dragOffset;
        }

        if (_dragging && Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            List<Vector3> dropCellWorldPos = GetCellWorldPosList();
            OnDropBlock?.Invoke(_cellSprite, dropCellWorldPos);
            transform.localScale /= 2;
            transform.position = _spawnPos;
        }
    }
    public void Init(BlockShapeData shape, Sprite cellSprite)
    {
        _grid = (bool[,])shape.GetGrid().Clone();
        
        _cellPosInShape = new List<Vector2Int>(shape.cells);
        _cellSprite = cellSprite;
        SpawnCells(_cellSprite);
        // Instantiate(_cellSpawnInBoard);
        // _cellSpawnInBoard.SetSprite(cellSprite);
        // _cellSpawnInBoard.gameObject.SetActive(false);
    }
    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    List<Vector3> GetCellWorldPosList()
    {
        List<Vector3> blockWorldPosList = new List<Vector3>();
        foreach (var cell in _cells)
        {
            blockWorldPosList.Add(cell.transform.position);
        }

        return blockWorldPosList;
    }
    // private void SpawnCells(Sprite cellSprite)
    // {
    //     foreach (var pos in _cells)
    //     {
    //         Cell cell = Instantiate(_cellPrefab, transform);
    //         cell.SetSprite(cellSprite);
    //         cell.transform.localPosition = new Vector3(cell.GetCellSize() * pos.x / 2, cell.GetCellSize() * pos.y / 2, 0);
    //         _cellColliders.Add(cell.GetComponent<Collider2D>());
    //     }
    // }
    private void SpawnCells(Sprite cellSprite)
    {
        float cellSize = _cellPrefab.GetCellSize();
        Vector2 center = GetShapeCenter();
        // _cellPrefab.SetSprite(cellSprite);
        
        foreach (var pos in _cellPosInShape)
        {
            Cell cell = Instantiate(_cellPrefab, transform);
            cell.SetSprite(cellSprite);

            Vector2 offset = pos - center;

            cell.transform.localPosition = new Vector3(offset.x * cellSize / 2f, offset.y * cellSize / 2f, 0);
            // cell.transform.localPosition = new Vector3(offset.x * cellSize, offset.y * cellSize, 0);
            _cellColliders.Add(cell.GetComponent<Collider2D>());
            _cells.Add(cell);
        }
    }

    Vector2 GetShapeCenter()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var pos in _cellPosInShape)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;

            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        return new Vector2(centerX, centerY);
    }

    public void SetSpawnPos(Vector3 pos)
    {
        _spawnPos = pos;
    }
    public void ReturnToPool()
    {
        if (_destroyOnReturn)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
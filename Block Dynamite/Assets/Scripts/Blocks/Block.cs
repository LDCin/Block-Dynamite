using System;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static event Action<Block> OnDropBlock;
    private int _id;
    public int ID
    {
        get => _id;
        set => _id = value;
    }
        
    [SerializeField] private bool _destroyOnReturn = false;
    private bool[,] _grid = new bool[8, 8];
    private List<Vector2Int> _cellPosInShape = new List<Vector2Int>();
    public List<Vector2Int> CellPosInShape => _cellPosInShape;
    private List<Cell> _cells = new List<Cell>();
    [SerializeField] private Cell _cellPrefab;
    private List<Collider2D> _cellColliders = new();
    private Sprite _cellSprite;

    public Sprite CellSprite => _cellSprite;

    private bool _dragging = false;
    [SerializeField] private Vector3 _dragOffset = new Vector3(0, 1.5f, 0);
    private Vector3 _spawnPos;
    [SerializeField] private int _normalSortingOrder = 0;
    [SerializeField] private int _dragSortingOrder = 100;

    private List<SpriteRenderer> _cellRenderers = new List<SpriteRenderer>();

    private void Start()
    {
        _normalSortingOrder = _cellPrefab.GetComponent<SpriteRenderer>().sortingOrder;
    }
    private void OnEnable()
    {
        RestoreSortingOrder();
    }

    void Update()
    {
        if (Time.timeScale <= 0f)
        {
            // Nếu đang kéo mà game bị pause thì trả block về trạng thái bình thường
            if (_dragging)
            {
                _dragging = false;
                transform.localScale /= 2;
                RestoreSortingOrder();
                transform.position = _spawnPos;
            }
            return;
        }
        
        Vector3 mousePos = GetMouseWorldPos();

        if (Input.GetMouseButtonDown(0) && !_dragging)
        {
            foreach (var col in _cellColliders)
            {
                if (col != null && col.OverlapPoint(mousePos))
                {
                    _dragging = true;
                    transform.localScale *= 2;
                    SetDragSortingOrder();
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
            // OnDropBlock?.Invoke(_cellSprite, dropCellWorldPos);
            OnDropBlock?.Invoke(this);
            transform.localScale /= 2;
            RestoreSortingOrder();
            
            transform.position = _spawnPos;
        }
    }
    public void Init(BlockShapeData shape, Sprite cellSprite, int id)
    {
        _grid = (bool[,])shape.GetGrid().Clone();

        _cellPosInShape = new List<Vector2Int>(shape.cells);
        _cellSprite = cellSprite;
        _id = id;
        SpawnCells(_cellSprite);
        // Instantiate(_cellSpawnInBoard);
        // _cellSpawnInBoard.SetSprite(cellSprite);
        // _cellSpawnInBoard.gameObject.SetActive(false);
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public List<Vector3> GetCellWorldPosList()
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

            SpriteRenderer spriteRenderer = cell.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                _cellRenderers.Add(spriteRenderer);
                spriteRenderer.sortingOrder = _normalSortingOrder;
            }
        }
    }

    private Vector2 GetShapeCenter()
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

    private void SetDragSortingOrder()
    {
        foreach (var spriteRenderer in _cellRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = _dragSortingOrder;
            }
        }
    }

    private void RestoreSortingOrder()
    {
        foreach (var spriteRenderer in _cellRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = _normalSortingOrder;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Boards
{
    public class Board : MonoBehaviour
    {
        private int cols = 8;
        public int Cols => cols;
        private int rows = 8;
        public int Rows => rows;
        private CellSlot[, ] _grid;
        public CellSlot[,] Grid => _grid;
        [SerializeField] private GameObject _gridComponent;
        [SerializeField] private Cell _cellPrefab;
        public Cell CellPrefab => _cellPrefab;

        [SerializeField] private CellSlot _cellSlotPrefab;
        // private SpriteRenderer _gridSpriteRenderer;
        private BoxCollider2D _gridSpriteCollider;
        private float _gridWidth;
        private float _gridHeight;
        private float _cellWidth;
        private float _cellHeight;
        private Vector2 _rootPos;

        private void Awake()
        {
            // _gridSpriteRenderer = _gridComponent.GetComponent<SpriteRenderer>();
            _gridSpriteCollider = _gridComponent.GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            InitGrid();
            InitBaseSizes();
        }

        private void InitGrid()
        {
            _grid = new CellSlot[cols, rows];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    CellSlot newCellSlot = Instantiate(_cellSlotPrefab, transform);
                    _grid[col, row] = newCellSlot;
                    _grid[col, row].IsOccupied = false;
                }
            }
        }

        private void InitBaseSizes()
        {
            _gridWidth = _gridSpriteCollider.bounds.size.x;
            _gridHeight = _gridSpriteCollider.bounds.size.y;
            Debug.Log("GridWidth: " + _gridWidth);
            Debug.Log("GridHeight: " + _gridHeight);
            _rootPos = _gridSpriteCollider.bounds.min;
            
            _cellWidth = _gridWidth / cols;
            _cellHeight = _gridHeight / rows;
        }

        public bool ConvertWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            float fx = (worldPos.x - _rootPos.x) / _cellWidth;
            float fy = (worldPos.y - _rootPos.y) / _cellHeight;

            int x = Mathf.FloorToInt(fx);
            int y = Mathf.FloorToInt(fy);

            bool isInside = x >= 0 && x < cols && y >= 0 && y < rows;
            gridPos = new Vector2Int(x, y);

            return isInside;
        }

        public Vector3 ConvertGridToWorld(Vector2Int gridPos)
        {
            float x = _rootPos.x + (gridPos.x + 0.5f) * _cellWidth;
            float y = _rootPos.y + (gridPos.y + 0.5f) * _cellHeight;

            return new Vector3(x, y, 0);
        }

        public bool IsOccupied(Vector2Int gridPos)
        {
            // return _grid[gridPos.x, gridPos.y].IsOccupied;
            return _grid[gridPos.x, gridPos.y].IsOccupied;
        }

        public void SetOccupied(Vector2Int gridPos)
        {
            _grid[gridPos.x, gridPos.y].IsOccupied = true;
        }

        public void SetCell(Vector2Int gridPos, Cell cell)
        {
            _grid[gridPos.x, gridPos.y].Cell = cell;
        }
        public bool GetRemainCellPosList(out List<Vector2Int> remainCellPosList)
        {
            remainCellPosList = new List<Vector2Int>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (!_grid[col, row].IsOccupied)
                    {
                        remainCellPosList.Add(new Vector2Int(col, row));
                    }
                }
            }

            return remainCellPosList.Count > 0;
        }
        // [SerializeField] private bool _showGridGizmos = true;
        // [SerializeField] private Color _gridLineColor = Color.white;
        //
        // private void OnDrawGizmos()
        // {
        //     if (!_showGridGizmos || _gridComponent == null) return;
        //
        //     var sr = _gridComponent.GetComponent<BoxCollider2D>();
        //     if (sr == null) return;
        //
        //     Bounds b = sr.bounds; // world bounds
        //     float cw = b.size.x / cols;
        //     float ch = b.size.y / rows;
        //
        //     Gizmos.color = _gridLineColor;
        //
        //     // Vertical lines
        //     for (int x = 0; x <= cols; x++)
        //     {
        //         float px = b.min.x + x * cw;
        //         Gizmos.DrawLine(new Vector3(px, b.min.y, 0f), new Vector3(px, b.max.y, 0f));
        //     }
        //
        //     // Horizontal lines
        //     for (int y = 0; y <= rows; y++)
        //     {
        //         float py = b.min.y + y * ch;
        //         Gizmos.DrawLine(new Vector3(b.min.x, py, 0f), new Vector3(b.max.x, py, 0f));
        //     }
        // }
    }
}

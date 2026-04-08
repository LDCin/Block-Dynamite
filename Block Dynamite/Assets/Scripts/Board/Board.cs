using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class Board : MonoBehaviour
    {
        private int cols = 8;
        private int rows = 8;
        private bool[, ] _grid;
        [SerializeField] private GameObject _gridComponent;
        [SerializeField] private Cell _cellPrefab;
        private SpriteRenderer _gridSpriteRenderer;
        private BoxCollider2D _gridSpriteCollider;
        private float _gridWidth;
        private float _gridHeight;
        private float _cellWidth;
        private float _cellHeight;
        private Vector2 _rootPos;

        private void Awake()
        {
            _gridSpriteRenderer = _gridComponent.GetComponent<SpriteRenderer>();
            _gridSpriteCollider = _gridComponent.GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            Block.OnDropBlock += SetBlock;
        }
        
        private void OnDisable()
        {
            Block.OnDropBlock -= SetBlock;
        }
        
        private void Start(){
            InitGrid();
            InitBaseSizes();
        }

        private void InitGrid()
        {
            _grid = new bool[cols, rows];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    _grid[col, row] = false;
                }
            }
        }

        private void InitBaseSizes()
        {
            // _gridWidth = _gridSpriteRenderer.sprite.bounds.size.x;
            // _gridHeight = _gridSpriteRenderer.sprite.bounds.size.y;
            // Debug.Log("GridWidth: " + _gridWidth);
            // Debug.Log("GridHeight: " + _gridHeight);
            // _rootPos = _gridSpriteRenderer.sprite.bounds.min;
            
            _gridWidth = _gridSpriteCollider.bounds.size.x;
            _gridHeight = _gridSpriteCollider.bounds.size.y;
            Debug.Log("GridWidth: " + _gridWidth);
            Debug.Log("GridHeight: " + _gridHeight);
            _rootPos = _gridSpriteCollider.bounds.min;
            
            _cellWidth = _gridWidth / cols;
            _cellHeight = _gridHeight / rows;
        }

        private bool ConvertWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            float fx = (worldPos.x - _rootPos.x) / _cellWidth;
            float fy = (worldPos.y - _rootPos.y) / _cellHeight;

            // Floor matches cell containment and is stable with ConvertGridToWorld (cell center positions).
            int x = Mathf.FloorToInt(fx);
            int y = Mathf.FloorToInt(fy);

            bool isInside = x >= 0 && x < cols && y >= 0 && y < rows;
            gridPos = new Vector2Int(x, y);

            return isInside;
        }

        private Vector3 ConvertGridToWorld(Vector2Int gridPos)
        {
            float x = _rootPos.x + (gridPos.x + 0.5f) * _cellWidth;
            float y = _rootPos.y + (gridPos.y + 0.5f) * _cellHeight;

            return new Vector3(x, y, 0);
        }

        public bool CanPlace(List<Vector3> cellPosCheckList, out List<Vector2Int> gridPosList)
        {
            gridPosList = new List<Vector2Int>();
            HashSet<Vector2Int> uniquePos = new HashSet<Vector2Int>();
            foreach (var pos in cellPosCheckList)
            {
                Vector2Int convertedPos;
                bool canConvert = ConvertWorldToGrid(pos, out convertedPos);
                gridPosList.Add(convertedPos);
                if (!canConvert)
                {
                    return false;
                }

                if (!uniquePos.Add(convertedPos))
                {
                    // Two cells from one block mapped to the same grid slot.
                    return false;
                }
            }
            foreach (var pos in gridPosList)
            {
                if (_grid[pos.x, pos.y])
                {
                    return false;
                }
            }

            return true;
        }

        private void SetBlock(Sprite cellSprite, List<Vector3> cellPosCheckList)
        {
            List<Vector2Int> gridPosList;
            if (!CanPlace(cellPosCheckList, out gridPosList))
            {
                Debug.Log("Can't Drop Block");
                return;
            }
            foreach (var pos in gridPosList)
            {
                Cell cell = Instantiate(_cellPrefab, transform);
                cell.SetSprite(cellSprite);
                cell.transform.position = ConvertGridToWorld(pos);
                _grid[pos.x, pos.y] = true;
            }
        }

        // Trong class Board (Assets/Scripts/Board/Board.cs)
        [SerializeField] private bool _showGridGizmos = true;
        [SerializeField] private Color _gridLineColor = Color.white;

        private void OnDrawGizmos()
        {
            if (!_showGridGizmos || _gridComponent == null) return;
        
            var sr = _gridComponent.GetComponent<BoxCollider2D>();
            if (sr == null) return;
        
            Bounds b = sr.bounds; // world bounds
            float cw = b.size.x / cols;
            float ch = b.size.y / rows;
        
            Gizmos.color = _gridLineColor;
        
            // Vertical lines
            for (int x = 0; x <= cols; x++)
            {
                float px = b.min.x + x * cw;
                Gizmos.DrawLine(new Vector3(px, b.min.y, 0f), new Vector3(px, b.max.y, 0f));
            }
        
            // Horizontal lines
            for (int y = 0; y <= rows; y++)
            {
                float py = b.min.y + y * ch;
                Gizmos.DrawLine(new Vector3(b.min.x, py, 0f), new Vector3(b.max.x, py, 0f));
            }
        }

    }
}

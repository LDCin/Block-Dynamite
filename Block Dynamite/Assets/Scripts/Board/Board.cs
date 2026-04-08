using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class Board : MonoBehaviour
    {
        private int cols = 8;
        private int rows = 8;
        private CellSlot[, ] _grid;
        [SerializeField] private GameObject _gridComponent;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private CellSlot _cellSlotPrefab;
        private SpriteRenderer _gridSpriteRenderer;
        private float _gridWidth;
        private float _gridHeight;
        private float _cellWidth;
        private float _cellHeight;
        private Vector2 _rootPos;

        private void Awake()
        {
            _gridSpriteRenderer = _gridComponent.GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            Block.OnDropBlock += SetBlock;
        }

        private void Start()
            _grid = new CellSlot[cols, rows];
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    CellSlot cellSlot = Instantiate(_cellSlotPrefab);
                    _grid[row, col] = cellSlot;
                }
            }
            _gridWidth = _gridSpriteRenderer.sprite.bounds.size.x;
            _gridHeight = _gridSpriteRenderer.sprite.bounds.size.y;
            _rootPos = _gridSpriteRenderer.sprite.bounds.min;

            _cellWidth = _gridWidth / 8;
            _cellHeight = _gridHeight / 8;
        }

        private bool ConvertWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            int x = Mathf.FloorToInt(worldPos.x - _rootPos.x);
            int y = Mathf.FloorToInt(worldPos.y - _rootPos.y);

            bool isInside = x >= 0 && x <= cols && y >= 0 && y <= rows;
            gridPos = new Vector2Int(x, y);

            return isInside;
        }

        private Vector3 ConvertGridToWorld(Vector2Int gridPos)
        {
            float x = _rootPos.x + gridPos.x * _cellWidth;
            float y = _rootPos.y + gridPos.y * _cellHeight;

            return new Vector3(x, y, 0);
        }

        public bool CanPlace(List<Vector3> cellPosCheckList, out List<Vector2Int> gridPosList)
        {
            gridPosList = new List<Vector2Int>();
            foreach (var pos in cellPosCheckList)
            {
                Vector2Int convertedPos;
                bool canConvert = ConvertWorldToGrid(pos, out convertedPos);
                gridPosList.Add(convertedPos);
                if (!canConvert)
                {
                    return false;
                }
            }
            foreach (var pos in gridPosList)
            {
                if (_grid[pos.x, pos.y].IsOccupied)
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
                _grid[pos.x, pos.y].IsOccupied = true;
            }
        }
    }
}
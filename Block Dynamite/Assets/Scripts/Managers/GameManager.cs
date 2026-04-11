using System.Collections.Generic;
using UnityEngine;
using Utils;
using Boards;
using UI;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Board _board;
        [SerializeField] private BlockSpawner _blockSpawner;
        private int _playableBlock;
        private int _remainBlock;
        private List<int> _validBlockPosList = new List<int>();
        private List<Block> _currentBlockList = new List<Block>();

        private void Awake()
        {
            BlockSpawner blockSpawner = Instantiate(_blockSpawner, transform);
            _blockSpawner = blockSpawner;
        }
        private void Start()
        {
            _playableBlock = _blockSpawner.SpawnableBlocks;
            _remainBlock = _playableBlock;
            _blockSpawner.SpawnRandomBlocks();
            _currentBlockList = _blockSpawner.CurrentBlocks;
        }
        private void OnEnable()
        {
            Block.OnDropBlock += HandleDropBlock;
        }
    
        private void OnDisable()
        {
            Block.OnDropBlock -= HandleDropBlock;
        }
    
        private void HandleDropBlock(Block block)
        {
            List<Vector3> cellPosCheckList = block.GetCellWorldPosList();
            HashSet<Vector2Int> gridPosList;
            if (!CanPlace(cellPosCheckList, out gridPosList))
            {
                Debug.Log("Can't Drop Block");
                return;
            }

            block.ReturnToPool();
            _remainBlock -= 1;
            if (_remainBlock <= 0)
            {
                _remainBlock = _playableBlock;
                RestockPlayableBlock();
                _currentBlockList = _blockSpawner.CurrentBlocks;
            }

            foreach (var pos in gridPosList)
            {
                Cell cell = Instantiate(_board.CellPrefab, transform);
                cell.SetSprite(block.CellSprite);
                cell.transform.position = _board.ConvertGridToWorld(pos);
                _board.SetOccupied(pos);
                _board.SetCell(pos, cell);
            }

            ClearRowOrColumn();
            
            if (!CanPlay())
            {
                Debug.Log("Game Over");
                gameOverPanel.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }

        private bool CanPlace(List<Vector3> cellPosCheckList, out HashSet<Vector2Int> gridPosList)
        {
            gridPosList = new HashSet<Vector2Int>();
            foreach (var pos in cellPosCheckList)
            {
                Vector2Int convertedPos;
                bool canConvert = _board.ConvertWorldToGrid(pos, out convertedPos);
                if (!canConvert)
                {
                    return false;
                }

                if (!gridPosList.Add(convertedPos))
                {
                    return false;
                }

                if (_board.IsOccupied(convertedPos))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanClearRowOrColumn(out HashSet<Vector2Int> scoredBlockPosList)
        {
            scoredBlockPosList = new HashSet<Vector2Int>();

            for (int row = 0; row < _board.Rows; row++)
            {
                List<Vector2Int> blockInRow = new List<Vector2Int>();
                for (int col = 0; col < _board.Cols; col++)
                {
                    if (!_board.IsOccupied(new Vector2Int(col, row)))
                    {
                        blockInRow.Clear();
                        break;
                    }
                    blockInRow.Add(new Vector2Int(col, row));
                }

                if (blockInRow.Count > 0)
                {
                    scoredBlockPosList.UnionWith(blockInRow);
                }
            }
            
            for (int col = 0; col < _board.Cols; col++)
            {
                List<Vector2Int> blockInCol = new List<Vector2Int>();
                for (int row = 0; row < _board.Rows; row++)
                {
                    if (!_board.IsOccupied(new Vector2Int(col, row)))
                    {
                        blockInCol.Clear();
                        break;
                    }
                    blockInCol.Add(new Vector2Int(col, row));
                }

                if (blockInCol.Count > 0)
                {
                    scoredBlockPosList.UnionWith(blockInCol);
                }
            }

            return scoredBlockPosList.Count > 0;
        }

        private void ClearRowOrColumn()
        {
            HashSet<Vector2Int> scoredBlockPosList;
            if (CanClearRowOrColumn(out scoredBlockPosList))
            {
                foreach (var pos in scoredBlockPosList)
                {
                    Destroy(_board.Grid[pos.x, pos.y].Cell.gameObject);
                    _board.Grid[pos.x, pos.y].Cell = null;
                    _board.Grid[pos.x, pos.y].IsOccupied = false;
                }
            }
        }
        
        private bool GetValidBlockPosList(out List<int> validBlockPosList, List<Block> checkedBlockList, bool requireActiveBlock)
        {
            HashSet<int> validIds = new HashSet<int>();
            List<Vector2Int> remainCellPosList;
            if (_board.GetRemainCellPosList(out remainCellPosList))
            {
                foreach (var remainCellPos in remainCellPosList)
                {
                    foreach (var block in checkedBlockList)
                    {
                        if (requireActiveBlock && !block.gameObject.activeInHierarchy)
                        {
                            continue;
                        }

                        bool isValid = true;
                        foreach (var offsetPos in block.CellPosInShape)
                        {
                            int x = remainCellPos.x + offsetPos.x;
                            int y = remainCellPos.y + offsetPos.y;
                            if (x < 0 || x >= _board.Cols || y < 0 || y >= _board.Rows || _board.IsOccupied(new Vector2Int(x, y)))
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid)
                        {
                            validIds.Add(block.ID);
                        }
                    }
                }
            }

            validBlockPosList = new List<int>(validIds);
            return validBlockPosList.Count > 0;
        }
        private bool CanPlay()
        {
            _currentBlockList = _blockSpawner.CurrentBlocks;
            if (GetValidBlockPosList(out _validBlockPosList, _currentBlockList, true))
            {
                return true;
            }

            return false;
        }

        private void RestockPlayableBlock()
        {
            _validBlockPosList.Clear();
            if (GetValidBlockPosList(out _validBlockPosList, _blockSpawner.BlockInPool, false))
            {
                _blockSpawner.SpawnBlockByID(_validBlockPosList);
            }
            else
            {
                _blockSpawner.SpawnRandomBlocks();
            }
        }
        
        // TEST
        public GameOverPanel gameOverPanel;
    }
}

using System.Collections.Generic;
using UnityEngine;
using Utils;
public class BlockPool : MonoBehaviour
{
    private List<BlockShapeData> _blockShapeList;
    private List<CellData> _cellDataList;
    private List<Block> _blockList;
    public List<Block> BlockList => _blockList;
    [SerializeField] private Block _blockPrefab;
    private void Awake()
    {
        _blockShapeList = new List<BlockShapeData>(Resources.LoadAll<BlockShapeData>(GameConfig.BLOCK_SHAPE_DATA_PATH));
        _cellDataList = new List<CellData>(Resources.LoadAll<CellData>(GameConfig.CELL_DATA_PATH));
        Debug.Log("Shape count: " + _blockShapeList.Count);
        Debug.Log("Cell count: " + _cellDataList.Count);
        _blockList = new List<Block>();
        InitBlockList();
    }
    private void InitBlockList()
    {
        _blockList = new List<Block>();

        foreach (CellData cellData in _cellDataList)
        {
            foreach (var blockShapeData in _blockShapeList)
            {
                Block block = Instantiate(_blockPrefab, transform);
                block.Init(blockShapeData, cellData.cellSprite, blockShapeData.id);
                _blockList.Add(block);
                block.gameObject.SetActive(false);
            }
        }
    }
    [ContextMenu("Get Blocks")]
    public Block GetRandomBlock()
    {
        int id = Random.Range(0, _blockList.Count);
        while (_blockList[id].gameObject.activeInHierarchy)
        {
            id = Random.Range(0, _blockList.Count);
        }
        return _blockList[id];
    }

    public Block GetBlockByID(int id)
    {
        Block deactiveBlock = GetRandomBlock();
        foreach (var block in _blockList)
        {
            if (!block.gameObject.activeInHierarchy && block.ID == id)
            {
                deactiveBlock = block;
            }
        }

        return deactiveBlock;
    }
}
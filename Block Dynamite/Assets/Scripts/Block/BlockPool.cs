using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    private List<BlockShapeData> _blockShapeDatas;
    private List<CellData> _cellDatas;
    private List<Block> _blocks;
    [SerializeField] private Block _blockPrefab;
    private void Awake()
    {
        _blockShapeDatas = new List<BlockShapeData>(Resources.LoadAll<BlockShapeData>(GameConfig.BLOCK_SHAPE_DATA_PATH));
        _cellDatas = new List<CellData>(Resources.LoadAll<CellData>(GameConfig.CELL_DATA_PATH));
        Debug.Log("Shape count: " + _blockShapeDatas.Count);
        Debug.Log("Cell count: " + _cellDatas.Count);
        _blocks = new List<Block>();
        InitBlockList();
    }
    private void InitBlockList()
    {
        _blocks = new List<Block>();

        foreach (CellData cellData in _cellDatas)
        {
            foreach (var blockShapeData in _blockShapeDatas)
            {
                Block block = Instantiate(_blockPrefab, transform);
                block.Init(blockShapeData, cellData.cellSprite);
                _blocks.Add(block);
                block.gameObject.SetActive(false);
            }
        }
    }
    [ContextMenu("Get Blocks")]
    public Block GetBlock()
    {
        int id = Random.Range(0, _blocks.Count);
        while (_blocks[id].gameObject.activeInHierarchy)
        {
            id = Random.Range(0, _blocks.Count);
        }
        return _blocks[id];
    }
}
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    private List<BlockShapeData> _blockShapeDatas;
    private List<CellData> _cellDatas;
    private List<Block> _blocks;
    private void Awake()
    {
        _blockShapeDatas = new List<BlockShapeData>(Resources.LoadAll<BlockShapeData>(GameConfig.BLOCK_SHAPE_DATA_PATH));
        _cellDatas = new List<CellData>(Resources.LoadAll<CellData>(GameConfig.CELL_DATA_PATH));
        _blocks = new List<Block>();
    }

    private void InitBlockList()
    {
        _blocks = new List<Block>();

        foreach (CellData cellData in _cellDatas)
        {
            foreach (var blockShapeData in _blockShapeDatas)
            {
                Block block = new Block();
                block.Init(blockShapeData, cellData.cellSprite);
                _blocks.Add(block);
            }
        }
    }
    public Block GetBlock()
    {
        
    }
    
}
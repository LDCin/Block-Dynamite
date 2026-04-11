using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private BlockPool _blockPool;
    public BlockPool BlockPool => _blockPool;
    private List<Block> _blockInPool = new List<Block>();
    public List<Block> BlockInPool => _blockInPool;
    
    [SerializeField] private List<Transform> _spawnPoses;
    [SerializeField] private float _spawnScale = 0.5f;
    private  List<Block> _currentBlocks = new List<Block>();

    public int SpawnableBlocks => _spawnPoses.Count;
    public List<Block> CurrentBlocks => _currentBlocks;

    private void Awake()
    {
        BlockPool blockPool = Instantiate(_blockPool, transform);
        _blockPool = blockPool;
    }

    private void Start()
    {
        _blockInPool = _blockPool.BlockList;
    }

    [ContextMenu("Spawn Block")]
    public void SpawnRandomBlocks()
    {
        _currentBlocks.Clear();

        for (int i = 0; i < SpawnableBlocks; i++)
        {
            Block block = _blockPool.GetRandomBlock();
            block.gameObject.SetActive(true);
            block.transform.localScale = Vector3.one * _spawnScale;
            block.transform.position = _spawnPoses[i].position;
            block.SetSpawnPos(_spawnPoses[i].position);
            _currentBlocks.Add(block);
        }
    }

    public void SpawnBlockByID(List<int> blockIDList)
    {
        _currentBlocks.Clear();

        if (blockIDList.Count == 0)
        {
            SpawnRandomBlocks();
            return;
        }

        for (int i = 0; i < SpawnableBlocks; i++)
        {
            int chosenId = blockIDList[Random.Range(0, blockIDList.Count)];
            Block block = _blockPool.GetBlockByID(chosenId);

            block.gameObject.SetActive(true);
            block.transform.localScale = Vector3.one * _spawnScale;
            block.transform.position = _spawnPoses[i].position;
            block.SetSpawnPos(_spawnPoses[i].position);
            _currentBlocks.Add(block);
        }
    }
}
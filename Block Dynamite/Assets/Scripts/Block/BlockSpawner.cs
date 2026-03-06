using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private BlockPool _blockPool;
    [SerializeField] private int _spawnQuantity = 3;
    [SerializeField] private List<Transform> _spawnPoses;

    private void Awake()
    {
        BlockPool blockPool = Instantiate(_blockPool, transform);
        _blockPool = blockPool;
    }
    private void Start()
    {
        SpawnBlocks();
    }
    [ContextMenu("Spawn Block")]
    public void SpawnBlocks()
    {
        for (int i = 0; i < 3; i++)
        {
            Block block = _blockPool.GetBlock();
            block.gameObject.SetActive(true);
            block.transform.localScale /= 2;
            block.transform.position = _spawnPoses[i].position;
            block.SetSpawnPos(_spawnPoses[i].position);
        }
    }
}
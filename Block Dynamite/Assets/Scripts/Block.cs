using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool[,] _grid = new bool[8, 8];
    private List<Vector2Int> _cells = new List<Vector2Int>();
    [SerializeField] private Cell _cellPrefab;
    public Block()
    {
        
    }
    public List<Vector2Int> GetCells()
    {
        return _cells;
    }
    public bool[,] GetGrid()
    {
        return _grid;
    }
    public void Init(BlockShapeData shape, Sprite cellSprite)
    {
        _grid = (bool[,])shape.GetGrid().Clone();
        _cells = new List<Vector2Int>(shape.cells);

        SpawnCells(cellSprite);
    }
    private void SpawnCells(Sprite cellSprite)
    {
        foreach (var pos in _cells)
        {
            Cell cell = Instantiate(_cellPrefab, transform);
            cell.SetSprite(cellSprite);
            cell.transform.localPosition = new Vector3(cell.GetCellSize() * pos.x, cell.GetCellSize() * pos.y, 0);
        }
    }

    public void ReturnToPool()
    {
        
    }
}
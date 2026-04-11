using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Block/Shape")]
public class BlockShapeData : ScriptableObject
{
    public int id;
    [SerializeField] private bool[] grid = new bool[64];
    public List<Vector2Int> cells = new List<Vector2Int>();

    public bool GetCellValue(int x, int y)
    {
        return grid[y * 8 + x];
    }

    public void SetCellValue(int x, int y, bool value)
    {
        grid[y * 8 + x] = value;
    }
    public bool[,] GetGrid()
    {
        bool[,] result = new bool[8, 8];

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                result[x, y] = GetCellValue(x, y);
            }
        }

        return result;
    }
    public void RebuildGridFromCells()  
    {  
        grid = new bool[64];  
  
        foreach (var pos in cells)  
        {  
            SetCellValue(pos.x, pos.y, true);  
        }  
    }
}

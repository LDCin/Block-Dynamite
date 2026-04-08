using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Rendering;

[CustomEditor(typeof(BlockShapeData))]
public class BlockShapeEditor : Editor
{
    private const int cellSize = 25;
    private const int boardSize = 8;
    public override void OnInspectorGUI()
    {
        BlockShapeData shape = (BlockShapeData)target;
        
        GUILayout.Label("Editor", EditorStyles.boldLabel);
        GUILayout.Space(5);

        DrawGrid(shape);
        
        GUILayout.Space(10);

        if (GUILayout.Button("Clear Cells"))
        {
            ClearCells(shape);
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Reload Cells"))
        {
            ReloadCells(shape);
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Default Data");
        DrawDefaultInspector();
    }
    private void DrawGrid(BlockShapeData shape)
    {
        for (int y = boardSize - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            
            for (int x = 0; x < boardSize; x++)
            {
                bool value = shape.GetCellValue(x, y);
                
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = value ? Color.green : Color.gray;
                
                if (GUILayout.Button("", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    shape.SetCellValue(x, y, !shape.GetCellValue(x, y));
                    UpdateCells(shape);
                    EditorUtility.SetDirty(shape);
                }

                GUI.backgroundColor = originalColor;
            }
            
            GUILayout.EndHorizontal();
        }
    }

    private void UpdateCells(BlockShapeData shape)
    {
        shape.cells.Clear();

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (shape.GetCellValue(x, y))
                {
                    minY = Mathf.Min(minY, y);
                    minX = Mathf.Min(minX, x);
                }
            }
        }

        if (minX == int.MaxValue)
        {
            return;
        }
        
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (shape.GetCellValue(x, y))
                {
                    shape.cells.Add(new Vector2Int(x - minX, y - minY));
                }
            }
        }
    }
    private void ClearCells(BlockShapeData shape)
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                shape.SetCellValue(x, y, false);
            }
        }
        
        shape.cells.Clear();
        
        EditorUtility.SetDirty(shape);
    }
    private void ReloadCells(BlockShapeData shape)
    {
        shape.RebuildGridFromCells();
        EditorUtility.SetDirty(shape);
        Repaint();
    }
} 
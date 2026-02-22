using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _cellSize = 1f;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetSprite(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }
}
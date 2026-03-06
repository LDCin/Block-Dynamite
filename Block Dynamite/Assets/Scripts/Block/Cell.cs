using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _cellSize = 1.05f;
    private string _spriteName;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetSprite(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
        _spriteName = newSprite.name;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }
}
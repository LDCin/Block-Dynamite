using System;
using UnityEngine;

namespace Board
{
    public class CellSlot : MonoBehaviour
    {
        [SerializeField] private Cell _cellPrefabs;
        // private 
        private BoxCollider2D _col;

        private void Awake()
        {
            _col = GetComponent<BoxCollider2D>();
        }
        
    }
}
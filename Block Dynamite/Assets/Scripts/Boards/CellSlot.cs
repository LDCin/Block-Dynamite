using System;
using UnityEngine;

namespace Boards
{
    public class CellSlot : MonoBehaviour
    {
        private Cell _cell;
        private bool _isOccupied = false;

        public bool IsOccupied
        {
            get => _isOccupied;
            set => _isOccupied = value;
        }

        public Cell Cell
        {
            get => _cell;
            set => _cell = value;
        }
    }
}
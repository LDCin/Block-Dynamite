using System;
using UnityEngine;

namespace Board
{
    public class CellSlot : MonoBehaviour
    {
        private bool _isOccupied = false;

        public bool IsOccupied
        {
            get => _isOccupied;
            set => _isOccupied = value;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Block/Library")]
public class BlockShapeLibrary : ScriptableObject
{
    [SerializeField] private List<BlockShapeData> shapes = new List<BlockShapeData>();
}

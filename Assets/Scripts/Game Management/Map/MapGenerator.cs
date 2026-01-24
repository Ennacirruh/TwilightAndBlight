using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Map
{
    public abstract class MapGenerator : ScriptableObject
    {
        public abstract void GenerateMap(Transform parent, ref Dictionary<Vector2Int, MapNode> mapList, ref Dictionary<MapNode, Vector2Int> lookupDict);
    }
}
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Map
{
    [CreateAssetMenu(menuName = "Mag Generator/Import Prefab")]
    public class MG_ImportPrefab : MapGenerator
    {
        [SerializeField] private GameObject prefab;
        public override void GenerateMap(Transform parent, ref Dictionary<Vector2Int, MapNode> mapList, ref Dictionary<MapNode, Vector2Int> lookupDict)
        {
            GameObject newMap = Instantiate(prefab);
        }
    }
}
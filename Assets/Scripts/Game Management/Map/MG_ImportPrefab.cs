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
            GameObject newMap = Instantiate(prefab, parent);
            MapNode[] nodes = newMap.GetComponentsInChildren<MapNode>();
            foreach (MapNode node in nodes)
            {
                if (node.enabled && node.gameObject.activeSelf)
                {
                    Vector2Int gridPos = node.PositionInMap;
                    mapList.Add(gridPos, node);
                    lookupDict.Add(node, gridPos);
                }
            }

        }
    }
}
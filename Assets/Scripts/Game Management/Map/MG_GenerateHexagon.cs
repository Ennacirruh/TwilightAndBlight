using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight.Map
{
    [CreateAssetMenu(menuName = "Mag Generator/Generate Hexagon")]
    public class MG_GenerateHexagon : MapGenerator
    {
        [SerializeField] private GameObject mapNodePrefab;
        [SerializeField] private int radius;
        [SerializeField] private Vector2 offsetPerNode;
        [SerializeField] private float heightRange;
        [SerializeField] private float heightRangeNoiseScale = 1f;
        [SerializeField] private float maxPercentAsSpawnNodes;
        [SerializeField] private float initialSpawnNodeChance;
        [SerializeField] private float subsequentSpawnNodeChanceModifier;

        private float spawnNodeChance;
        private int spawnNodeLimit;

        public override void GenerateMap(Transform parent, ref Dictionary<Vector2Int, MapNode> map, ref Dictionary<MapNode, Vector2Int> lookupDict)
        {
            spawnNodeChance = initialSpawnNodeChance;
            spawnNodeLimit = (int)((GameUtility.FactorialInt(radius) * 6) * maxPercentAsSpawnNodes);
            CreateNode(Vector3Int.zero, parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(1,0,0), parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(-1,0,0), parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(0,1,0), parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(0,-1,0), parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(0,0,1), parent, ref map, ref lookupDict);
            GenerateNodesInDirection(new Vector3Int(0,0,-1), parent, ref map, ref lookupDict);
            
            List<MapNode> nodes = new List<MapNode>(MapManager.Instance.GetMapNodes());
            for (int i = 0; i < spawnNodeLimit; i++)
            {
                if (Random.Range(0, 1f) <= spawnNodeChance)
                {
                    int index = Random.Range(0, nodes.Count);
                    MapNode mapNode = nodes[index];
                    nodes.RemoveAt(index);
                    mapNode.SetSpawnNode(true);
                    spawnNodeChance *= subsequentSpawnNodeChanceModifier;
                    int filterRoll = Random.Range(-1, CombatManager.Instance.GetCombatTeams().Count);
                    if (filterRoll >= 0)
                    {
                        mapNode.AssignSpanNodeFilter(CombatManager.Instance.GetCombatTeams()[filterRoll]);
                    }
                }
            }
        }

        private void GenerateNodesInDirection(Vector3Int direction, Transform parent, ref Dictionary<Vector2Int, MapNode> map, ref Dictionary<MapNode, Vector2Int> lookupDict)
        {
            Vector3Int swizzel = new Vector3Int(-direction.z, direction.x, direction.y);
            for (int i = 1; i < radius; i++)
            {
                CreateNode(direction * i, parent, ref map, ref lookupDict);
                for(int j = 1;  j <= radius - 1 - i; j++)
                {
                    CreateNode((swizzel * j) + (direction * i), parent, ref map, ref lookupDict);

                }
            }
        }

        private void CreateNode(Vector3Int offset, Transform parent, ref Dictionary<Vector2Int, MapNode> map, ref Dictionary<MapNode, Vector2Int> lookupDict)
        {
            //Debug.Log($"Offset: {offset} -> Pos: {MapManager.Instance.GetRealativePosition(Vector2Int.zero, offset)}");
            GameObject newNode = Instantiate(mapNodePrefab, parent.transform);
            Vector2Int gridPos = MapManager.Instance.GetRealativePosition(Vector2Int.zero, offset);
            float xPos = offsetPerNode.x * gridPos.x + (offsetPerNode.x / 2f) * (Mathf.Abs(gridPos.y % 2) + 1);
            float zPos = offsetPerNode.y * gridPos.y;
            float yPos =  Mathf.PerlinNoise((xPos * heightRangeNoiseScale) + 10000, (zPos * heightRangeNoiseScale) + 10000) * heightRange;//(Mathf.Abs(offset.x) + Mathf.Abs(offset.y) + Mathf.Abs(offset.z)) * -0.2f; 
            newNode.transform.position = new Vector3(xPos, yPos, zPos);
            newNode.name = newNode.name + $"{gridPos.x}_{gridPos.y}";
            MapNode mapNode = newNode.GetComponent<MapNode>();
            mapNode.PositionInMap = gridPos;
            map.Add(gridPos, mapNode);
            lookupDict.Add(mapNode, gridPos);

        }
    }
}
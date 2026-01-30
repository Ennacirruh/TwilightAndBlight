using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using Valve.VR;
namespace TwilightAndBlight.Map
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
        [SerializeField] private Color genericIndicator;
        [SerializeField] private Color altGenericIndicator;
        [SerializeField] private Color warningIndicator;
        [SerializeField] private Color validIndicator;
        [SerializeField] private Color invalidIndicator;
        public static MapManager Instance { get { return instance; } }
        [SerializeField] private MapGenerator mapGenerator;
        private Dictionary<Vector2Int, MapNode> map = new Dictionary<Vector2Int, MapNode>();
        private Dictionary<MapNode, Vector2Int> mapNodePositionLookup = new Dictionary<MapNode, Vector2Int>();
        private Dictionary<CombatTeam, List<MapNode>> spawnPoints = new Dictionary<CombatTeam, List<MapNode>>();
        private List<MapNode> universalSpawnPoints = new List<MapNode>();
        private HashSet<MapNode> overlayNodes = new HashSet<MapNode>();
        private Dictionary<MapNode, IndicatorType> currentHighlightDict = new Dictionary<MapNode, IndicatorType>();
        public const float gridPosMultiplier = 0.85f;
        public const float gridSizeMultiplier = 0.42f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            mapGenerator.GenerateMap(transform, ref map, ref mapNodePositionLookup);
            List<CombatTeam> teams = CombatManager.Instance.GetCombatTeams();
            foreach (CombatTeam team in teams)
            {
                spawnPoints.Add(team, new List<MapNode>());
            }
            foreach (MapNode node in map.Values)
            {
                if (node.IsSpawnNode())
                {
                    CombatTeam filter = node.GetSpawnFilter();
                    if (filter != null)
                    {
                        spawnPoints[filter].Add(node);
                    }
                    else
                    {
                        universalSpawnPoints.Add(node);
                        foreach (CombatTeam team in teams)
                        {
                            spawnPoints[team].Add(node);
                        }
                    }
                }
            }
        }
        public List<MapNode> GetMapNodes()
        {
            return new List<MapNode>(map.Values);
        }
        public Dictionary<Vector2Int, MapNode> GetMap()
        {
            return map;
        }
        public MapNode GetNode(Vector2Int pos)
        {
            if (map.ContainsKey(pos))
            {
                return map[pos];
            }
            return null;
        }
        public Vector2Int GetNodePosition(MapNode mapNode)
        {
            return mapNodePositionLookup[mapNode];
        }
        public MapNode GetRealativeNode(Vector2Int originPos, int i, int j, int k)
        {
            Vector2Int targetPos = GetRealativePosition(originPos, i, j, k);
            return GetNode(targetPos);
        }
        public MapNode GetRealativeNode(Vector2Int originPos, Vector3Int offset)
        {
            Vector2Int targetPos = GetRealativePosition(originPos, offset);
            return GetNode(targetPos);
        }
        public MapNode GetRealativeNode(MapNode originPos, int i, int j, int k)
        {
            return GetRealativeNode(mapNodePositionLookup[originPos], i, j, k);
        }
        public MapNode GetRealativeNode(MapNode originPos, Vector3Int offset)
        {
            return GetRealativeNode(mapNodePositionLookup[originPos], offset.x, offset.y, offset.z);
        }
        public void ResetHighlight()
        {
            foreach (MapNode node in currentHighlightDict.Keys)
            {
                node.ResetMaterial();
            }
            foreach (MapNode node in overlayNodes)
            {
                node.ResetMaterial();
            }
            currentHighlightDict.Clear();
            overlayNodes.Clear();
        }
        public void HighlightNodesAsOverlay(MapNode node, IndicatorType indicatorType, bool erasePrevious = true)
        {
            HighlightNodesAsOverlay(new List<MapNode>() { node }, indicatorType, erasePrevious);
        }
        public void HighlightNodesAsOverlay(List<MapNode> nodes, IndicatorType indicatorType, bool erasePrevious = true)
        {
            if (erasePrevious)
            {
                foreach (MapNode node in overlayNodes)
                {
                    if (node != null)
                    {
                        if (currentHighlightDict.ContainsKey(node))
                        {
                            ColorNode(node, currentHighlightDict[node]);
                        }
                        else
                        {
                            node.ResetMaterial();
                        }
                    }
                }
                overlayNodes.Clear();
            }
            foreach (MapNode node in nodes)
            {
                if (node != null)
                {
                    overlayNodes.Add(node);
                    ColorNode(node, indicatorType);
                }
            }
        }
        public void HighlightNodes(MapNode node, IndicatorType indicatorType)
        {
            HighlightNodes(new List<MapNode>() { node }, indicatorType);
        }
        public void HighlightNodes(List<MapNode> nodes, IndicatorType indicatorType)
        {
            foreach (MapNode node in nodes)
            {
                if (node != null)
                {
                    bool changeColor = false;
                    if (currentHighlightDict.ContainsKey(node))
                    {
                        if (currentHighlightDict[node] != indicatorType)
                        {
                            changeColor = true;
                            currentHighlightDict[node] = indicatorType;
                        }
                    }
                    else
                    {
                        currentHighlightDict.Add(node, indicatorType);
                        changeColor = true;
                    }
                    if (changeColor)
                    {
                        ColorNode(node, indicatorType);
                    }

                }
            }
        }

        public List<MapNode> GetSpawnNodes(CombatTeam filter = null)
        {
            if (filter == null || !spawnPoints.ContainsKey(filter))
            {
                return universalSpawnPoints;
            }
            return spawnPoints[filter];
        }
        public Vector2Int GetRealativePosition(Vector2Int origin, Vector3Int offset)
        {
            return GetRealativePosition(origin, offset.x, offset.y, offset.z);
        }
        public Vector2Int GetRealativePosition(Vector2Int origin, int i, int j, int k)
        {
            return new Vector2Int(i + Mathf.FloorToInt((j + Mathf.Abs((origin.y) % 2)) / 2f) - Mathf.FloorToInt((k + Mathf.Abs((j + k + origin.y) % 2)) / 2f), j + k) + origin;
        }

        private void ColorNode(MapNode node, IndicatorType indicator)
        {
            switch (indicator)
            {
                case IndicatorType.Generic:
                    node.ColorMaterial(genericIndicator);
                    break;
                case IndicatorType.Valid:
                    node.ColorMaterial(validIndicator);
                    break;
                case IndicatorType.Invalid:
                    node.ColorMaterial(invalidIndicator);
                    break;
                case IndicatorType.Warnign:
                    node.ColorMaterial(warningIndicator);
                    break;
                case IndicatorType.AltGeneric:
                    node.ColorMaterial(altGenericIndicator);
                    break;
            }
        }
        public static bool IsValidNode(MapNode node)
        {
            if (node == null) return false;
            if (node.IsOccupied()) return false;

            return true;
        }
        public static bool IsValidNeighboringNode(MapNode originNode, MapNode neighborNode)
        {
            if (neighborNode == null || originNode == null) return false;
            if (neighborNode.IsOccupied()) return false;
            if (!CanMantle(originNode, neighborNode)) return false;

            return true;
        }
        public static bool WillTakeFallDamge(MapNode originNode, MapNode neighborNode)
        {
            return (originNode.transform.position.y - neighborNode.transform.position.y) >= GameManager.Instance.FallDamageThreshold;
        }
        public static bool CanMantle(MapNode originNode, MapNode neighborNode)
        {
            return neighborNode.transform.position.y - originNode.transform.position.y <= GameManager.Instance.TerrainMantleThreshold;
        }
        public static float FallDistance(MapNode originNode, MapNode neighborNode)
        {
            return originNode.transform.position.y - neighborNode.transform.position.y;

        }
        public HashSet<MapNode> GetNodesWithinRange(MapNode originNode, int range, MapNodeConditional condition = null)
        {
            HashSet<MapNode> returnSet = new HashSet<MapNode>();
            returnSet.Add(originNode);

            Vector3Int direction = new Vector3Int(0, 0, -1);
            for(int i = 0; i < 6; i++)
            {
                direction = new Vector3Int(-direction.z, direction.x, direction.y);
                for(int j = 1; j < range; j++)
                {
                    Vector3Int rotDirection = new Vector3Int(-direction.z, direction.x, direction.y); 
                    for (int k = 0; k <= range - j - 1; k++)
                    {
                        Vector3Int offset = direction * j + (rotDirection * k);
                        MapNode newNode = GetRealativeNode(originNode, offset);
                        if (newNode != null)
                        {
                            bool valid = true;
                            if(condition != null)
                            {
                                valid &= condition.Invoke(newNode);
                            }
                            if(valid) returnSet.Add(newNode);
                        }
                    }
                }
            
            }

            return returnSet;
        }
        public HashSet<(MapNode,MapNode)> GetNodesWithinMoveLimit(MapNode originNode, int maxMoves, MapNodeParentConditional condition = null)
        {
            HashSet<(MapNode, int)> nodesToEvaluate = new HashSet<(MapNode, int)>();
            Dictionary<MapNode, int> evaluatedNodes = new Dictionary<MapNode, int>();
            Dictionary<MapNode, MapNode> parentNodes = new Dictionary<MapNode, MapNode>();
            HashSet<MapNode> nodesInRange = new HashSet<MapNode>(); ;
            nodesToEvaluate.Add((originNode, 0));
            while (nodesToEvaluate.Count > 0)
            {
                (MapNode, int) nodeInEvaluation = nodesToEvaluate.First();
                nodesToEvaluate.Remove(nodeInEvaluation);
                if (nodeInEvaluation.Item2 < maxMoves)
                {
                    nodesInRange.Add(nodeInEvaluation.Item1);

                    Vector3Int direction = new Vector3Int(0, 0, -1);
                    for (int i = 0; i < 6; i++)
                    {
                        direction = new Vector3Int(-direction.z, direction.x, direction.y);
                        MapNode candidate = GetRealativeNode(nodeInEvaluation.Item1, direction);
                        if (candidate != null)
                        {
                            bool valid = true;
                            if (condition != null)
                            {
                                valid &= condition.Invoke(nodeInEvaluation.Item1, candidate);
                            }
                            if (valid)
                            {
                                int newDistance = nodeInEvaluation.Item2 + 1;

                                if (evaluatedNodes.ContainsKey(candidate)) // node previously evaluated
                                {
                                    if (evaluatedNodes[candidate] > newDistance) //shorter path discovered, re-evaluate for chance of new nodes
                                    {
                                        nodesToEvaluate.Add((candidate, newDistance));
                                        evaluatedNodes[candidate] = newDistance;
                                        parentNodes[candidate] = nodeInEvaluation.Item1;
                                    }
                                }
                                else //new node encounter
                                {
                                    evaluatedNodes.Add(candidate, newDistance);
                                    nodesToEvaluate.Add((candidate, newDistance));
                                    parentNodes.Add(candidate, nodeInEvaluation.Item1);
                                }
                            }
                        }
                    }
                }
            }
            HashSet<(MapNode, MapNode)> returnSet = new HashSet<(MapNode, MapNode)>();
            foreach(MapNode node in nodesInRange)
            {
                MapNode parent;
                if (parentNodes.ContainsKey(node))
                {
                    parent = parentNodes[node];
                }
                else
                {
                    parent = null;
                }
                returnSet.Add((node, parent));
            }
            return returnSet;
        }
    }
}
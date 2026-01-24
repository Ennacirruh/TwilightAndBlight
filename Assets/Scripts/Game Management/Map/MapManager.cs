using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using Valve.VR;
namespace TwilightAndBlight.Map
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
        [SerializeField] private Color genericIndicator;
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


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //mapGenerator.GenerateMap(transform, ref map);
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
            foreach(MapNode node in map.Values)
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
                        foreach(CombatTeam team in teams)
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
        public void HighlightNodesAsOverlay(MapNode node, IndicatorType indicatorType)
        {
            HighlightNodesAsOverlay(new List<MapNode>() { node }, indicatorType);
        }
        public void HighlightNodesAsOverlay(List<MapNode> nodes, IndicatorType indicatorType)
        {
            foreach(MapNode node in overlayNodes)
            {
                if(node != null)
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
            foreach (MapNode node in nodes)
            {
                if(node != null)
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
                        if(currentHighlightDict[node] != indicatorType)
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
            if(filter == null || !spawnPoints.ContainsKey(filter))
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
            return new Vector2Int(i + Mathf.FloorToInt((j + Mathf.Abs((origin.y) % 2)) /2f) - Mathf.FloorToInt((k+Mathf.Abs((j+k+origin.y)%2))/2f), j + k) + origin;
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
            }
        }
    }
}
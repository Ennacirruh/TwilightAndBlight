using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TwilightAndBlight.Collections;
using TwilightAndBlight.Map;
using Unity.XR.OpenVR;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
namespace TwilightAndBlight.AI
{
    public static class AStarNavigation
    {
        private static Dictionary<MapNode, NavData> navDataDictionary = new Dictionary<MapNode, NavData>();

        public static List<MapNode> GetShortestPath(MapNode start, MapNode end, int maxMoves)
        {
            maxMoves++;
            if (start == null || end == null)
            {
                return null;
            }
            navDataDictionary.Clear();
            
            SortedSet<MapNode> open = new SortedSet<MapNode>(new MapNodeNavComparer());
            HashSet<MapNode> closed = new HashSet<MapNode>();

            open.Add(start);
            navDataDictionary.Add(start, new NavData(0, 0, null));
            int abortPathfindCounter = 0;
            while (open.Count > 0)
            {
                MapNode currentNode = open.Min;
                open.Remove(currentNode);
                abortPathfindCounter++;
                if (abortPathfindCounter == 10000)
                {
                    Debug.LogError($"Suspected Infinate Loop! Start {start.name}, End {end.name}, maxMoves {maxMoves}");
                    return new List<MapNode>();
                }


                if (currentNode == end)
                {
                    List<MapNode> path = ReconstructPath(end);
                    return path; //destination reached
                }
                Vector3Int direction = new Vector3Int(0, 0, -1);
                for (int i = 0; i < 6; i++)
                {
                    direction = new Vector3Int(-direction.z, direction.x, direction.y);

                    MapNode neighboringNode = MapManager.Instance.GetRealativeNode(currentNode, direction);

                    if (MapManager.IsValidNeighboringNode(currentNode, neighboringNode))
                    {
                        float score = EvaluateNodeScore(currentNode, neighboringNode, end, navDataDictionary[currentNode].score);
                        int distance = navDataDictionary[currentNode].distance + currentNode.GetMovesToLeaveNode();

                        bool skipNode = false;

                        if (open.Contains(neighboringNode, new MapNodeEqualityComparison()) || closed.Contains(neighboringNode))
                        {
                            if (navDataDictionary[neighboringNode].score <= score || navDataDictionary[neighboringNode].distance <= distance) skipNode = true;
                        }

                        if (!skipNode)
                        {
                            if (distance <= maxMoves)
                            {
                                NavData newData = new NavData(distance, score, currentNode);
                                if (navDataDictionary.ContainsKey(neighboringNode))
                                {
                                    open.Remove(neighboringNode);
                                    navDataDictionary[neighboringNode] = newData;
                                }
                                else
                                {
                                    navDataDictionary.Add(neighboringNode, newData);
                                }

                                closed.Remove(neighboringNode);
                                open.Add(neighboringNode);
                            }
                            else { Debug.Log(neighboringNode); }
                        }
                    }
                }

                closed.Add(currentNode);
            }


            return new List<MapNode>();
        }
        public static List<MapNode> ReconstructPath(MapNode end)
        {
            MapNode currentNode = end;
            List<MapNode> path = new List<MapNode>();
            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = navDataDictionary[currentNode].parent;
            }
            path.Reverse();
            return path;
        }
        public static List<MapNode> GetShortestPath(Vector2Int start, Vector2Int end, int maxMoves)
        {
            navDataDictionary.Clear();

            MapNode startNode = MapManager.Instance.GetNode(start);
            MapNode endNode = MapManager.Instance.GetNode(end);
            if (startNode == null || endNode == null)
            {
                return null;
            }
            return GetShortestPath(startNode, endNode, maxMoves);
        }

        public static List<MapNode> GetShortestPath(MapNode start, Vector3Int offset, int maxMoves)
        {
            MapNode endNode = MapManager.Instance.GetRealativeNode(start, offset);
            if (start == null || endNode == null)
            {
                return null;
            }
            return GetShortestPath(start, endNode, maxMoves);
        }

        public static List<MapNode> GetShortestPath(Vector2Int start, Vector3Int offset, int maxMoves)
        {
            MapNode startNode = MapManager.Instance.GetNode(start);
            MapNode endNode = MapManager.Instance.GetRealativeNode(start, offset);
            if (startNode == null || endNode == null)
            {
                return null;
            }
            return GetShortestPath(startNode, endNode, maxMoves);
        }

        private static float EvaluateNodeScore(MapNode originNode, MapNode evaluating, MapNode finalDestination, float costUntilNow)
        {
            Vector2 currentVector2 = new Vector2(evaluating.transform.position.x, evaluating.transform.position.z);
            Vector2 endVector2 = new Vector2(finalDestination.transform.position.x, finalDestination.transform.position.z);
            float magnitude = (endVector2 - currentVector2).magnitude;
            float value = Mathf.Pow(magnitude, 0.20f);
            if (originNode != null)
            {
                if (MapManager.WillTakeFallDamge(originNode, evaluating))
                {
                    value *= value;
                }
                value *= originNode.GetMovesToLeaveNode();
            }
            value *= evaluating.GetPathFindingScoreModifier();

            return value + costUntilNow;
        }


        private class MapNodeNavComparer : IComparer<MapNode>
        {
            public int Compare(MapNode x, MapNode y)
            {
                if(x == y)
                {
                    return 0;
                }
                if (navDataDictionary.ContainsKey(x) && navDataDictionary.ContainsKey(y))
                {
                    if (navDataDictionary[x].score > navDataDictionary[y].score)
                    {
                        return 1;
                    }
                }
                return -1;
            }
        }
        //if (x.transform.position.y < y.transform.position.y)
        //{
        //    return -1;
        //}
        //if (x.transform.position.y > y.transform.position.y)
        //{
        //    return 1;
        //}
        //if (x.PositionInMap.x < y.PositionInMap.x)
        //{
        //    return -1;
        //}
        //if (x.PositionInMap.x > y.PositionInMap.x)
        //{
        //    return 1;
        //}
        //if (x.PositionInMap.y < y.PositionInMap.y)
        //{
        //    return -1;
        //}
        //if (x.PositionInMap.y > y.PositionInMap.y)
        //{
        //    return 1;
        //}
        //if (navDataDictionary[x].score > navDataDictionary[y].score)
        //{
        //    return 1;
        //}
        private class MapNodeEqualityComparison : IEqualityComparer<MapNode>
        {
            public bool Equals(MapNode x, MapNode y)
            {
                return x == y;
            }

            public int GetHashCode(MapNode obj)
            {
                return obj.GetHashCode();
            }
        }
        private struct NavData
        {
            public int distance;
            public float score;
            public MapNode parent;
            public NavData(int distance, float score, MapNode parent)
            {
                this.distance = distance;
                this.score = score;
                this.parent = parent;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
using TwilightAndBlight.AI;
using TMPro;
namespace TwilightAndBlight.Ability
{
    public class EA_Move : EntityAbility
    {
        [SerializeField] private float agilityPerMoveSpeed = 100;
        [SerializeField] private int baseMoveSpeed = 3;
        
        private HashSet<MapNode> validNodesInRange = new HashSet<MapNode>();
        private int totalMoves;
        

        protected override void OnEnable()
        {
            combatEntity.RegisterFreeAction(this);
        }
        protected override void OnDisable()
        {
            combatEntity.UnregisterFreeAction(this);

        }
        private bool IsValidPathNode(MapNode parent, MapNode neighbor)
        {
            return MapManager.IsValidNeighboringNode(parent, neighbor);
        }
        
        public override void HighlightAbility(MapNode targetingOrigin)
        {
        
            MapManager.Instance.ResetHighlight();
            MapNode currentNode = combatEntity.GetCurrentMapNode();
            totalMoves = GetMoveSpeed();
            HashSet<(MapNode,MapNode)> nodesInRange = MapManager.Instance.GetNodesWithinMoveLimit(currentNode, totalMoves, IsValidPathNode);
            validNodesInRange.Clear();
            foreach ((MapNode, MapNode) set in nodesInRange)
            {   
                validNodesInRange.Add(set.Item1);
            }

            
            foreach (MapNode node in validNodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
            }
            
            if (IsValidTarget(targetingOrigin))
            {
                List<MapNode> path = AStarNavigation.GetShortestPath(combatEntity.GetCurrentMapNode(), targetingOrigin, totalMoves);
                
                if (path.Count > 0)
                {
                    MapNode parent = path[0];
                    MapManager.Instance.HighlightNodesAsOverlay(path, IndicatorType.Valid);
                    foreach (MapNode node in path)
                    {
                        if (MapManager.WillTakeFallDamge(parent, node))
                        {
                            MapManager.Instance.HighlightNodesAsOverlay(node, IndicatorType.Warning, false);
                        }
                        parent = node;
                    }
                }
                else
                {
                    MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
                }
            }
            else
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
            }

                
        }
        
        public override bool IsValidTarget(MapNode targetNode)
        {
            if (validNodesInRange.Contains(targetNode))
            {
                return base.IsValidTarget(targetNode);
            }
            return false;
        }
        

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            MapNode currentNode = combatEntity.GetCurrentMapNode();
            int moves = GetMoveSpeed();
            float speed = 5f + (entityStats.Agility / 500f);
            List<MapNode> path = AStarNavigation.GetShortestPath(currentNode, targetingOrigin, moves);
            if (path != null)
            {
                targetingOrigin.FollowPathMovePattern(combatEntity, path, speed);
            }
            yield return new WaitUntil(() => {return !targetingOrigin.MoveInProgress(); });
            
            EndAbility(targetingOrigin);
        }

        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            return new Dictionary<string, string>() {
                {"agilitypermovespeed", agilityPerMoveSpeed.ToString()},
                {"basemovespeed", baseMoveSpeed.ToString()},
                {"totalmovespeed", GetMoveSpeed().ToString()}
            };
        }
        private int GetMoveSpeed()
        {
            return Mathf.FloorToInt(baseMoveSpeed + ((agilityPerMoveSpeed != 0)? (entityStats.Agility / agilityPerMoveSpeed) : 0));
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            targetFilter = AbilityTarget.EmptyNode;
        }

        //public override bool HasValidTargetInRange()
        //{
        //    Vector3Int direction = new Vector3Int(0, 0, -1);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        direction = new Vector3Int(-direction.z, direction.x, direction.y);
        //        MapNode node = MapManager.Instance.GetRealativeNode(combatEntity.GetCurrentMapNode(), direction);
        //        if (node != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}

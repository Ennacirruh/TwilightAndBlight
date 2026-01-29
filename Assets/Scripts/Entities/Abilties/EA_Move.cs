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
        private bool nodeRecalulationQueued = true;
        private int totalMoves;
        

        protected override void OnEnable()
        {
            GameEvents.OnTurnStart += QueueNodeRecalculation;
        }
        protected override void OnDisable()
        {
            GameEvents.OnTurnStart -= QueueNodeRecalculation;

        }
        private bool IsValidPathNode(MapNode parent, MapNode neighbor)
        {
            return MapManager.IsValidNeighboringNode(parent, neighbor);
        }
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            if (nodeRecalulationQueued)
            {
                MapManager.Instance.ResetHighlight();
                nodeRecalulationQueued = false;
                MapNode currentNode = combatEntity.GetCurrentMapNode();
                totalMoves = GetMoveSpeed();
                HashSet<(MapNode,MapNode)> nodesInRange = MapManager.Instance.GetNodesWithinMoveLimit(currentNode, totalMoves, IsValidPathNode);
                validNodesInRange.Clear();
                foreach ((MapNode, MapNode) set in nodesInRange)
                {   
                    validNodesInRange.Add(set.Item1);
                }

            }
            foreach (MapNode node in validNodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
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
                            MapManager.Instance.HighlightNodesAsOverlay(node, IndicatorType.Warnign, false);
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
            float speed = 1f + (entityStats.Agility / 50f);
            List<MapNode> path = AStarNavigation.GetShortestPath(currentNode, targetingOrigin, moves);
            if (path != null)
            {
                targetingOrigin.FollowPathMovePattern(combatEntity, path, speed);
            }
            yield return new WaitUntil(() => { return !currentNode.MoveInProgress(); });
            EndAbility(targetingOrigin);
        }

        protected override Dictionary<string, string> GetStringConversionTable()
        {
            return new Dictionary<string, string>() {
                {"agilitypermovespeed", agilityPerMoveSpeed.ToString()},
                {"basemovespeed", baseMoveSpeed.ToString()},
                {"totalmovespeed", GetMoveSpeed().ToString()}
            };
        }
        private int GetMoveSpeed()
        {
            return Mathf.FloorToInt(baseMoveSpeed + (entityStats.Agility / agilityPerMoveSpeed));
        }
        private void QueueNodeRecalculation(CombatEntity combatEntity)
        {
            if (combatEntity = this.combatEntity)
            {
                nodeRecalulationQueued = true;
            }
        }
        //protected override bool HighlightConditions(MapNode parentNode, MapNode node)
        //{
        //    if (node != null && MapManager.IsValidNeighboringNode(parentNode, node))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        protected override void OnValidate()
        {
            base.OnValidate();
            targetFilter = AbilityTarget.EmptyNode;
        }
    }
}

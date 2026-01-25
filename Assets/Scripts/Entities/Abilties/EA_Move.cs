using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
using System.Linq;
using TwilightAndBlight.AI;
namespace TwilightAndBlight.Ability
{
    public class EA_Move : EntityAbility
    {
        [SerializeField] private float agilityPerMoveSpeed = 100;
        [SerializeField] private int baseMoveSpeed = 3;
        private HashSet<(MapNode, int)> nodesToEvaluate = new HashSet<(MapNode, int)>();
        private Dictionary<MapNode, int> evaluatedNodes = new Dictionary<MapNode, int>();
        private HashSet<MapNode> nodesInRange = new HashSet<MapNode>();
        private bool nodeRecalulationQueued = true;
        private int totalMoves;
        

        private void OnEnable()
        {
            GameEvents.OnTurnStart += QueueNodeRecalculation;
        }
        private void OnDisable()
        {
            GameEvents.OnTurnStart -= QueueNodeRecalculation;

        }
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            if (nodeRecalulationQueued)
            {
                MapManager.Instance.ResetHighlight();
                nodeRecalulationQueued = false;
                MapNode currentNode = combatEntity.GetCurrentMapNode();
                totalMoves = GetMoveSpeed();
                nodesToEvaluate.Clear();
                evaluatedNodes.Clear();
                nodesInRange.Clear();
                nodesToEvaluate.Add((currentNode, 0));
                while (nodesToEvaluate.Count > 0)
                {
                    (MapNode, int) nodeInEvaluation = nodesToEvaluate.First();
                    nodesToEvaluate.Remove(nodeInEvaluation);
                    if (nodeInEvaluation.Item2 < totalMoves)
                    {
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(1, 0, 0));  //  right
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(-1, 0, 0));  //  left
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(0, 1, 0));  //  upper right
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(0, -1, 0));  //  lower left
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(0, 0, 1));  //  upper left
                        EvaluateNode(nodeInEvaluation.Item1, nodeInEvaluation.Item2, new Vector3Int(0, 0, -1));  //  lower right
                    }
                }
                

            }
            foreach (MapNode node in nodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
            }
            
            if (IsValidTarget(targetingOrigin))
            {
                List<MapNode> path = AStarNavigation.GetShortestPath(combatEntity.GetCurrentMapNode(), targetingOrigin, totalMoves);
                if (path.Count > 0)
                {
                    MapManager.Instance.HighlightNodesAsOverlay(path, IndicatorType.Valid);
                }
            }
            else
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
            }

                
        }
        
        public override bool IsValidTarget(MapNode targetNode)
        {
            if (nodesInRange.Contains(targetNode))
            {
                return base.IsValidTarget(targetNode);
            }
            return false;
        }
        private void EvaluateNode(MapNode parentNode, int parentDistance, Vector3Int targetNodeOffset)
        {
            MapNode target = MapManager.Instance.GetRealativeNode(parentNode, targetNodeOffset.x, targetNodeOffset.y, targetNodeOffset.z);
            int newDistance = parentDistance + 1;
            if (target != null && MapManager.IsValidNeighboringNode(parentNode, target)) // node exists
            {
                if (evaluatedNodes.ContainsKey(target)) // node previously evaluated
                {
                    //Debug.Log($"Distance Old: {evaluatedNodes[target]} vs New: {newDistance}");
                    if (evaluatedNodes[target] > newDistance) //shorter path discovered, re-evaluate for chance of new nodes
                    {
                        nodesToEvaluate.Add((target, newDistance));
                        evaluatedNodes[target] = newDistance;
                        MapManager.Instance.HighlightNodes(target, IndicatorType.Warnign);

                    }
                }
                else //new node encounter
                {
                    nodesInRange.Add(target);
                    evaluatedNodes.Add(target, newDistance);
                    nodesToEvaluate.Add((target, newDistance));
                    MapManager.Instance.HighlightNodes(target, IndicatorType.Valid);
                }
            }
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
        protected override void OnValidate()
        {
            base.OnValidate();
            targetFilter = AbilityTarget.EmptyNode;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericDamage : EA_GenericAbilityShape
    {
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            HashSet<MapNode> nodes = GetTargetingNodes(targetingOrigin);
            foreach (MapNode node in nodes)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
            }
        }

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            throw new System.NotImplementedException();
        }
    }
}
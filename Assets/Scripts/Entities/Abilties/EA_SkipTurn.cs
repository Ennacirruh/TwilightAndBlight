using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_SkipTurn : EntityAbility
    {
        public override void HighlightAbility(MapNode targetingOrigin)
        {

        }

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            yield return null;
            EndAbility(targetingOrigin);
        }
        public override bool IsValidAbilityCast(MapNode targetNode)
        {
            return true;
        }
        public override bool IsValidTarget(MapNode targetNode)
        {
            return true;
        }

        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            return new Dictionary<string, string>();
        }
    }
}

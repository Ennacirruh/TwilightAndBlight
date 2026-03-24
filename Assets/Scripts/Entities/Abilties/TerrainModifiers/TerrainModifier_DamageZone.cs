using TwilightAndBlight.Events;
using UnityEngine;
namespace TwilightAndBlight.Map.Modifier
{

    public class TerrainModifier_DamageZone : TerrainModifier
    {
        public float TerrainDamage {  get; set; }
        public float TerrainPercentArmorPen { get; set; } = .5f;
        public float TerrainFlatArmorPen { get; set; } = 5f;

        public DamageType DamageType { get; set; } = DamageType.Fire;
        protected override void OnEnterTerrain(CombatEntityMapNodeInteractionCallback callback)
        {
            if (callback.mapNode == parentNode)
            {
                DamageEntity(callback.entity);
            }

        }

        protected override void OnExitTerrain(CombatEntityMapNodeInteractionCallback callback)
        {

        }

        protected override void OnTurnEnd(CombatEntityActionCallback callback)
        {

        }

        protected override void OnTurnStart(CombatEntityActionCallback callback)
        {
            if (callback.entity.GetCurrentMapNode() == parentNode)
            {
                DamageEntity(callback.entity);
            }
        }
        public void DamageEntity(CombatEntity entity)
        {
            entity.DamageEntity(null, TerrainDamage, DamageType, TerrainPercentArmorPen, TerrainFlatArmorPen);

        }
    }
}

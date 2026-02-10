using UnityEngine;
namespace TwilightAndBlight.Ability.Infliction
{
    public abstract class Affliction : MonoBehaviour
    {
        protected CombatEntity combatEntity;
        protected virtual void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();
            if(combatEntity == null)
            {
                Destroy(this);
            }
        }
    }
}
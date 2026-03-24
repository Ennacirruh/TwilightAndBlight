using UnityEngine;
namespace TwilightAndBlight.Ability.Passive
{
    [RequireComponent(typeof(CombatEntity))]

    public abstract class Passive : MonoBehaviour, IDescriptable
    {
        [SerializeField] protected string passiveName;
        [SerializeField] [TextArea] protected string description;


        public string GetName()
        {
            return passiveName;

        }

        public string GetDescription()
        {
            return description;

        }
    }
}

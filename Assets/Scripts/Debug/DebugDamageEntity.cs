using TwilightAndBlight;
using UnityEngine;
[RequireComponent(typeof(EmptyCombatEntity))]
public class DebugDamageEntity : MonoBehaviour
{
    private CombatEntity sourceEntity;
    [SerializeField] private CombatEntity targetEntity;
    [SerializeField] private bool damageEntity;
    [SerializeField] private float attack;
    [SerializeField] private float flatPen;
    [SerializeField] private float percentPen;
    [SerializeField] private DamageType damageType;
    private void Awake()
    {
        sourceEntity = GetComponent<CombatEntity>();
    }
    private void Update()
    {
        if (damageEntity)
        {
            damageEntity = false;
            targetEntity.DamageEntity(sourceEntity, attack, damageType, percentPen, flatPen);
        }
    }
}

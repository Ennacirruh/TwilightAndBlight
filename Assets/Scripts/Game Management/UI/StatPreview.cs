using TMPro;
using TwilightAndBlight;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private StatType statType;
    [SerializeField] private EntityStats entityStats;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private DescriptionView descriptionView;
    public void DistplayStat(EntityStats stats, StatType type, DescriptionView description)
    {
        statType = type;
        entityStats = stats;
        descriptionView = description;

        text.text = type.ToString() + ": " + entityStats.GetStat(statType).Value;

    }
    
    public string GetStatDescription(StatType statType)
    {
        switch (statType)
        {
            case StatType.Power:
                return "The primary driver of damage.";
            case StatType.Fortitude:
                return "Decreases damage taken.";
            case StatType.Agility:
                return "Increases the speed and rate at which an entity takes its turn.";
            case StatType.Constition:
                return "Determines max health.";
            case StatType.Intelligence:
                return "Increases crit power of damage, heal and shield abilities. (Shield crit not yet implemented)";
            case StatType.Wisdom:
                return "Stat not yet implemented.";
            case StatType.Dexterity:
                return "Increases resistance to fall damage.";
            case StatType.Reflexes:
                return "Decreases the crit chance of incoming damage, and can increase the crit chance of incoming healing or shielding.";
            case StatType.Cunning:
                return "Increases crit chance of damage, heal, and shield abilities. (Shield crit not yet implemented)";
            case StatType.Discipline:
                return "Increases the range of damage, healing and shield rolls in an entities favor.";
            case StatType.FlatArmorPen:
                return "Ignore a flat ammount of fortitude.";
            case StatType.PercentArmorPen:
                return "Ignores a percentage of an entities fortitude.";
            case StatType.Spirit:
                return "Determines max mana.";
            case StatType.Endurance:
                return "Determines max stamina.";
            case StatType.Vitality:
                return "Determines the rate at which health is regenerated.";
            case StatType.Tenacity:
                return "Determines the rate at which stamina is regenerated.";
            case StatType.Effervescence:
                return "Determines the rate at which mana is regenerated.";
            case StatType.Compassion:
                return "The primary driver of healing.";
            case StatType.Mettle:
                return "The primary driver of shielding";
            case StatType.Transendance:
                return "Not yet implemented.";
            default: return "Description Not Implemented";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionView.SetDescription(GetStatDescription(statType));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionView.PreviewDefault();
    }
}

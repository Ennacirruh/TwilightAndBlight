using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TwilightAndBlight.Ability;
using TwilightAndBlight;

public class AbilityPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image abilityIcon;
    [SerializeField] private DescriptionView descriptionView;
    [SerializeField] private Button button;
    private EntityAbility currentDisplay;
    private CombatEntity combatEntity;
    private void Awake()
    {
       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionView.PreviewDescriptable(currentDisplay, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionView.PreviewDefault();
    }


    public void DisplayAbility(EntityAbility ability, CombatEntity owner, DescriptionView descriptionView)
    {
        this.descriptionView = descriptionView;
        currentDisplay = ability;
        combatEntity = owner;
        abilityIcon.sprite = ability.GetAbilityIcon();
        button.interactable = ability.CanAffordAbility();
    }


    public void SelectAbility()
    {
        combatEntity.SetSelectedAbility(currentDisplay);
    }
}

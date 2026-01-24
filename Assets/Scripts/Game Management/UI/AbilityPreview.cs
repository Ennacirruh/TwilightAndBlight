using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TwilightAndBlight.Ability;
using TwilightAndBlight;

public class AbilityPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image abilityIcon;
    [SerializeField] private RectTransform descriptionBox;
    [SerializeField] private TextMeshProUGUI descritpionText;
    private EntityAbility currentDisplay;
    private CombatEntity combatEntity;
    public Vector2 offset;
    private void Awake()
    {
       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionBox.gameObject.SetActive(true);
        descriptionBox.localPosition = offset;
        descriptionBox.SetParent(UIManager.Instance.GetCanvas());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseAbilityDescription();
    }


    public void DisplayAbility(EntityAbility ability, CombatEntity owner)
    {
        currentDisplay = ability;
        combatEntity = owner;
        abilityIcon.sprite = ability.GetAbilityIcon();
        descritpionText.text = ability.GetAbilityName() + "\n" + ability.GetAbilityDescription();
    }
    public void CloseAbilityDescription()
    {
        descriptionBox.SetParent(transform);
        descriptionBox.gameObject.SetActive(false);
    }
    public void SelectAbility()
    {
        combatEntity.SetSelectedAbility(currentDisplay);
    }
}

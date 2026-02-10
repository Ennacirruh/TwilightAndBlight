using UnityEngine;
namespace TwilightAndBlight
{
    public class EntityResourceBars : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private GameObject manaBarPrefab;
        [SerializeField] private GameObject staminaBarPrefab;
        [SerializeField] private float healthBarVertOffset;
        [SerializeField] private float staminaBarVertOffset;
        [SerializeField] private float manaBarVertOffset;
        [SerializeField] private RectTransform canvasParent;
        private CombatEntity target;
        private ResourceBarController healthController;
        private ResourceBarController manaController;
        private ResourceBarController staminaController;
        private void Awake()
        {
            target = GetComponent<CombatEntity>();
        }
        private void Start()
        {
            GameObject resourceBar = Instantiate(healthBarPrefab, canvasParent);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * healthBarVertOffset;
            healthController = resourceBar.GetComponent<ResourceBarController>();

            resourceBar = Instantiate(manaBarPrefab, canvasParent);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * manaBarVertOffset;
            manaController = resourceBar.GetComponent<ResourceBarController>();

            resourceBar = Instantiate(staminaBarPrefab, canvasParent);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * staminaBarVertOffset;
            staminaController = resourceBar.GetComponent<ResourceBarController>();
        }

        private void OnEnable()
        {
            GameEvents.OnHealthChange += UpdateHealthBar;
            GameEvents.OnManaChange += UpdateManaBar;
            GameEvents.OnStaminaChange += UpdateStaminaBar;
        }

        private void OnDisable()
        {
            GameEvents.OnHealthChange -= UpdateHealthBar;
            GameEvents.OnManaChange -= UpdateManaBar;
            GameEvents.OnStaminaChange -= UpdateStaminaBar;

        }
        private void UpdateResourceBar(ResourceBarController controller, float max, float current)
        {            
            float t = current / max;
            controller.SetBarProgress(t);
        }
        private void UpdateHealthBar(CombatEntity entity, float difference)
        {
            if (entity == target)
            {
                UpdateResourceBar(healthController, entity.MaxHealth, entity.Health);
            }
        }
        private void UpdateStaminaBar(CombatEntity entity, float difference)
        {
            if (entity == target)
            {
                UpdateResourceBar(staminaController, entity.MaxStamina, entity.Stamina);
            }
        }
        private void UpdateManaBar(CombatEntity entity, float difference)
        {
            if (entity == target)
            {
                UpdateResourceBar(manaController, entity.MaxMana, entity.Mana);
            }
        }
    }
}

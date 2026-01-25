using UnityEngine;
namespace TwilightAndBlight
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject resourceBarPrefab;
        [SerializeField] private RectTransform canvasParent;
        private CombatEntity target;
        private ResourceBarController controller;
        private void Awake()
        {
            target = GetComponent<CombatEntity>();
        }
        private void Start()
        {
            GameObject resourceBar = Instantiate(resourceBarPrefab, canvasParent);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero;
            controller = resourceBar.GetComponent<ResourceBarController>();
        }

        private void OnEnable()
        {
            GameEvents.OnHealthChange += UpdateResourceBar;
        }

        private void OnDisable()
        {
            GameEvents.OnHealthChange -= UpdateResourceBar;

        }
        private void UpdateResourceBar(CombatEntity entity, float difference)
        {
            if (entity == target)
            {
                float Max = entity.MaxHealth;
                float current = entity.Health;
                float t = current / Max;
                controller.SetBarProgress(t);
            }
        }
    }
}

using UnityEngine;
namespace TwilightAndBlight
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject resourceBarPrefab;
        [SerializeField] private RectTransform canvasParent;
        private ResourceBarController controller;
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
            float Max = entity.MaxHealth;
            float current = entity.Health;
            float t = current / Max;    
            controller.SetBarProgress(t);
        }
    }
}

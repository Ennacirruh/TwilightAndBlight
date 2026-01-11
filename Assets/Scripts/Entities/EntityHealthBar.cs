using UnityEngine;
namespace TwilightAndBlight
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject resourceBarPrefab;
        [SerializeField] private Vector2 barOffsetPosition;
        private ResourceBarController controller;
        private void Start()
        {
            GameObject resourceBar = Instantiate(resourceBarPrefab, GameManager.Instance.GetMainCanvas().transform);
            resourceBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)barOffsetPosition);
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

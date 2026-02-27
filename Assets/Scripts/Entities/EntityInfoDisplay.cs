using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using TwilightAndBlight.Events;
namespace TwilightAndBlight
{
    public class EntityInfoDisplay : MonoBehaviour
    {
        private static GameObject healthBarPrefab;
        private static GameObject manaBarPrefab;
        private static GameObject staminaBarPrefab;
        private static GameObject textObjPrefab;
        [SerializeField] private float healthBarVertOffset = 20;
        [SerializeField] private float staminaBarVertOffset = 10;
        [SerializeField] private float manaBarVertOffset = 0;
        private RectTransform targetCanvas;
        private List<GameObject> textObjects = new List<GameObject>();
        private List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>();
        private HashSet<int> availableIndexes = new HashSet<int>();
        private CombatEntity combatEntity;

        
        private ResourceBarController healthController;
        private ResourceBarController manaController;
        private ResourceBarController staminaController;

        private void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();
            
            targetCanvas = (RectTransform)GetComponentInChildren<Canvas>().transform;
            if (healthBarPrefab == null) { healthBarPrefab =  Resources.Load<GameObject>("Indicators/HealthBar");}
            if (manaBarPrefab == null) { manaBarPrefab = Resources.Load<GameObject>("Indicators/ManaBar"); }
            if (staminaBarPrefab == null) { staminaBarPrefab = Resources.Load<GameObject>("Indicators/StaminaBar"); }
            if (textObjPrefab == null) { textObjPrefab = Resources.Load<GameObject>("Indicators/Floatingtext"); }
        }
        private void Start()
        {
            GameObject resourceBar = Instantiate(healthBarPrefab, targetCanvas);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * healthBarVertOffset;
            healthController = resourceBar.GetComponent<ResourceBarController>();
            UpdateHealthBar(new CombatResourceChangeActionCallback(combatEntity, 0));

            resourceBar = Instantiate(manaBarPrefab, targetCanvas);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * manaBarVertOffset;
            manaController = resourceBar.GetComponent<ResourceBarController>();
            UpdateManaBar(new CombatResourceChangeActionCallback(combatEntity, 0));


            resourceBar = Instantiate(staminaBarPrefab, targetCanvas);
            ((RectTransform)resourceBar.transform).anchoredPosition = Vector2.zero + Vector2.up * staminaBarVertOffset;
            staminaController = resourceBar.GetComponent<ResourceBarController>();
            UpdateStaminaBar(new CombatResourceChangeActionCallback(combatEntity, 0));

        }
        private void OnEnable()
        {
            GameEvents.OnEntiyDamaged += DisplayDamageText;
            GameEvents.OnEntityHealthReplenished += DisplayHealText;
            GameEvents.OnEntityHealthDrained += DisplayDamageText;
            GameEvents.OnHealthChange += UpdateHealthBar;
            GameEvents.OnManaChange += UpdateManaBar;
            GameEvents.OnStaminaChange += UpdateStaminaBar;
            GameEvents.OnEntityStaminaReplenished += DisplayStaminaText;
            GameEvents.OnEntityManaReplenished += DisplayManaText;
        }
        private void OnDisable()
        {
            GameEvents.OnEntiyDamaged -= DisplayDamageText;
            GameEvents.OnEntityHealthReplenished -= DisplayHealText;
            GameEvents.OnEntityHealthDrained -= DisplayDamageText;
            GameEvents.OnHealthChange -= UpdateHealthBar;
            GameEvents.OnManaChange -= UpdateManaBar;
            GameEvents.OnStaminaChange -= UpdateStaminaBar;
            GameEvents.OnEntityStaminaReplenished -= DisplayStaminaText;
            GameEvents.OnEntityManaReplenished -= DisplayManaText;
        }
        private void UpdateResourceBar(ResourceBarController controller, float max, float current)
        {
            controller.SetBarProgress(current, max);
        }
        private void UpdateHealthBar(CombatResourceChangeActionCallback callback)
        {
            if (callback.entity == combatEntity)
            {
                UpdateResourceBar(healthController, callback.entity.MaxHealth, callback.entity.Health);
            }
        }
        private void UpdateStaminaBar(CombatResourceChangeActionCallback callback)
        {
            if (callback.entity == combatEntity)
            {
                UpdateResourceBar(staminaController, callback.entity.MaxStamina, callback.entity.Stamina);
            }
        }
        private void UpdateManaBar(CombatResourceChangeActionCallback callback)
        {
            if (callback.entity == combatEntity)
            {
                UpdateResourceBar(manaController, callback.entity.MaxMana, callback.entity.Mana);
            }
        }
        public void DisplayFloatingText(string textToDisplay, FloatingTextSettings textSettings)
        {
            StartCoroutine(AnimateFloatingText(textToDisplay, textSettings));
        }
        public void DisplayDamageText(DamageEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                DisplayCritableText(callback.postMitigationDamage, callback.crit, GameManager.Instance.DamageColorOverTime, GameManager.Instance.DamageCritColorOverTime);
            }
        }
        
        public void DisplayHealText(ReplenishEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                DisplayCritableText(callback.recoveryValue, callback.crit, GameManager.Instance.HealColorOverTime, GameManager.Instance.HealCritColorOverTime);
            }
        }
        public void DisplayStaminaText(ReplenishEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                DisplayCritableText(callback.recoveryValue, callback.crit, GameManager.Instance.RecoverStaminaColorOverTime, GameManager.Instance.RecoverStaminaCritColorOverTime);
            }
        }
        public void DisplayManaText(ReplenishEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                DisplayCritableText(callback.recoveryValue, callback.crit, GameManager.Instance.RecoverManaColorOverTime, GameManager.Instance.RecoverManaCritColorOverTime);
            }
        }
        private void DisplayCritableText(float value, bool crit, Gradient baseColor, Gradient critColor)
        {
            FloatingTextSettings settings = GameManager.Instance.FloatingTextSettings;
            FontStyles style = settings.StyleSettings;
            if (crit)
            {
                settings.ColorOverLifetime = baseColor;
                settings.StyleSettings = FontStyles.Bold & FontStyles.Italic;
            }
            else
            {
                settings.ColorOverLifetime = critColor;
            }
            DisplayFloatingText(Mathf.CeilToInt(value).ToString(), settings);
            settings.StyleSettings = style;
        }
        public void DisplayDamageText(DrainEntityResourceInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                FloatingTextSettings settings = GameManager.Instance.FloatingTextSettings;
                settings.ColorOverLifetime = GameManager.Instance.DamageColorOverTime;
                DisplayFloatingText(Mathf.CeilToInt(callback.resourceDrain).ToString(), GameManager.Instance.FloatingTextSettings);
            }
        }

        private IEnumerator AnimateFloatingText(string textToDisplay, FloatingTextSettings textSettings)
        {
            Vector2 velocity = textSettings.InitialVelocity;
            Vector2 initialPosition = textSettings.InitialPosition;
            Vector2 acceleration = textSettings.Acceleration;
            float angularVelocity = textSettings.InitialAngularVelocity;
            float initialRotation = textSettings.InitialRotation;
            float duration = textSettings.TextDuration;
            float angularAcceleration = textSettings.AngularAcceleration;
            float initialTextSize = textSettings.InitialTextSize;
            Gradient colorOverLifetime = textSettings.ColorOverLifetime;
            FontStyles style = textSettings.StyleSettings;

            int index = GetAvailableTextObjectIndex();
            GameObject textObject = textObjects[index];
            RectTransform textTransform = (RectTransform)textObject.transform;
            TextMeshProUGUI text = textComponents[index];
            text.color = colorOverLifetime.Evaluate(0);
            text.fontStyle = style;
            text.fontSize = initialTextSize;
            text.text = textToDisplay;
            textObject.SetActive(true);
            textSettings.GenerateSettingInstance();
            textTransform.anchoredPosition = initialPosition;
            textTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, initialRotation));

            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;

                velocity += acceleration * Time.deltaTime;
                textTransform.anchoredPosition += velocity * Time.deltaTime;

                angularVelocity += angularAcceleration * Time.deltaTime;
                textTransform.localRotation = Quaternion.Euler(0, 180, textTransform.localRotation.eulerAngles.z + angularVelocity);

                Color newColor = colorOverLifetime.Evaluate(t);
                newColor.a = textSettings.OpacityOverLifetime.Evaluate(t);
                text.color = newColor;
                float newTextSize = initialTextSize * textSettings.SizeOverLifetime.Evaluate(t);
                text.fontSize = newTextSize;
                yield return null;
            }
            textObject.SetActive(false);
            availableIndexes.Add(index);
        }
        public int GetAvailableTextObjectIndex()
        {
            int index = 0;
            if (availableIndexes.Count == 0)
            {
                GameObject newInstance = Instantiate(textObjPrefab, targetCanvas);
                TextMeshProUGUI textComponent = newInstance.GetComponent<TextMeshProUGUI>();
                textObjects.Add(newInstance);
                textComponents.Add(textComponent);
                index = textObjects.Count - 1;
            }
            else
            {
                index = availableIndexes.First();
                availableIndexes.Remove(index);
            }
            return index;
        }

    }
}


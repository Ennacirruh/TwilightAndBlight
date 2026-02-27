using TwilightAndBlight.Collections;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
namespace TwilightAndBlight
{
    public class GameManager : MonoBehaviour
    {

        private static GameManager instance;
        [SerializeField] private float turnThreshold = 10000;
        [SerializeField] private float resourceInteractionVarianceRange = 0.25f;
        [SerializeField] private float fallDamageThreshold = 3f;
        [SerializeField] private float terrainMantleThreshold = 2f;
        [SerializeField] private float percentHealthDamageOnFallPerMeter = 0.01f;
        [SerializeField] private float flatDamageOnFallPerMeter = 15f;
        [SerializeField] private float fallDamagePercentArmorPen = 0.5f;
        [SerializeField] private float minimumTerrainHeight = -10.0f;
        [SerializeField] private float maximumTerrainHeight = 25.0f;
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private int maxAstra = 1;
        [SerializeField] private FloatingTextSettings floatingTextSettings;
        [SerializeField] private Gradient damageColorOverTime;
        [SerializeField] private Gradient damageCritColorOverTime;
        [SerializeField] private Gradient healColorOverTime;
        [SerializeField] private Gradient healCritColorOverTime;
        [SerializeField] private Gradient recoverStaminaColorOverTime;
        [SerializeField] private Gradient recoverStaminaCritColorOverTime;
        [SerializeField] private Gradient recoverManaColorOverTime;
        [SerializeField] private Gradient recoverManaCritColorOverTime;
        public float TurnThreshold { get { return turnThreshold; } }
        public float ResourceInteractionVarianceRange { get { return resourceInteractionVarianceRange; } }
        public float FallDamageThreshold { get { return fallDamageThreshold; } }
        public float TerrainMantleThreshold { get { return terrainMantleThreshold; } }
        public float PercentHealthDamageOnFallPerMeter { get { return percentHealthDamageOnFallPerMeter; } }
        public float FlatDamageOnFallPerMeter { get { return flatDamageOnFallPerMeter; } }
        public float FallDamagePercentArmorPen { get { return fallDamagePercentArmorPen; } }
        public float MinimumTerrainHeight { get { return minimumTerrainHeight; } }
        public float MaximumTerrainHeight { get { return maximumTerrainHeight; } }
        public float Gravity { get { return gravity; } }
        public float MaxAstra { get { return maxAstra; } }
        public FloatingTextSettings FloatingTextSettings { get { return floatingTextSettings; } }
        public Gradient DamageColorOverTime {  get { return damageColorOverTime; } }
        public Gradient DamageCritColorOverTime {  get { return damageCritColorOverTime; } }
        public Gradient HealColorOverTime {  get { return healColorOverTime; } }
        public Gradient HealCritColorOverTime {  get { return healCritColorOverTime; } }
        public Gradient RecoverStaminaColorOverTime { get { return recoverStaminaColorOverTime; } }
        public Gradient RecoverStaminaCritColorOverTime { get { return recoverStaminaCritColorOverTime; } }
        public Gradient RecoverManaColorOverTime { get { return recoverManaColorOverTime; } }
        public Gradient RecoverManaCritColorOverTime { get { return recoverManaCritColorOverTime; } }

        public static GameManager Instance { get { return instance; } }
        [SerializeField] private Canvas mainCanvas;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(mainCanvas);
            }
            else
            {
                Destroy(this);
            }
        }

        public Canvas GetMainCanvas()
        {
            return mainCanvas;
        }

    }
}
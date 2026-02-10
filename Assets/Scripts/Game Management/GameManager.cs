using TwilightAndBlight.Collections;
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
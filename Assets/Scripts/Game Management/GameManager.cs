using TwilightAndBlight.Collections;
using UnityEngine;
namespace TwilightAndBlight
{
    public class GameManager : MonoBehaviour
    {

        private static GameManager instance;
        [SerializeField] private float turnThreshold = 10000;
        [SerializeField] private float damageVarianceRange = 0.25f;
        [SerializeField] private float fallDamageThreshold = 3f;
        [SerializeField] private float terrainMantleThreshold = 2f;
        [SerializeField] private float percentHealthDamageOnFallPerMeter = 0.01f;
        [SerializeField] private float flatDamageOnFallPerMeter = 15f;
        [SerializeField] private float fallDamagePercentArmorPen = 0.5f;
        public float TurnThreshold { get { return turnThreshold; } }
        public float DamageVarianceRange { get { return damageVarianceRange; } }
        public float FallDamageThreshold { get { return fallDamageThreshold; } }
        public float TerrainMantleThreshold { get { return terrainMantleThreshold; } }
        public float PercentHealthDamageOnFallPerMeter { get { return percentHealthDamageOnFallPerMeter; } }
        public float FlatDamageOnFallPerMeter { get { return flatDamageOnFallPerMeter; } }
        public float FallDamagePercentArmorPen { get { return fallDamagePercentArmorPen; } }
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
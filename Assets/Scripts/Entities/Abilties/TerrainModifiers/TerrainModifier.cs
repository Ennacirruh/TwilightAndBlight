using TwilightAndBlight.Events;
using UnityEngine;

namespace TwilightAndBlight.Map.Modifier
{

    public abstract class TerrainModifier : MonoBehaviour
    {
        [SerializeField] private GameObject modifierVisualPrefab;
        [SerializeField] private bool initializeOnAwake;
        protected GameObject modifierVisual;
        protected MapNode parentNode;
        private float scaleMemory;
        protected virtual void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize(modifierVisualPrefab);
            }
        }
        protected virtual void OnEnable()
        {
            GameEvents.OnTurnStart += OnTurnStart;
            GameEvents.OnTurnEnd += OnTurnEnd;
            GameEvents.OnNodeEntered += OnEnterTerrain;
            GameEvents.OnNodeExited += OnExitTerrain;
        }
        protected virtual void OnDisable()
        {
            GameEvents.OnTurnStart -= OnTurnStart;
            GameEvents.OnTurnEnd -= OnTurnEnd;
            GameEvents.OnNodeEntered -= OnEnterTerrain;
            GameEvents.OnNodeExited -= OnExitTerrain;
        }
        private void OnDestroy()
        {
            Destroy(modifierVisual);
        }
        public virtual void Initialize(GameObject visualPrefab)
        {
            parentNode = GetComponent<MapNode>();
            modifierVisual = Instantiate(visualPrefab, parentNode.transform);
            UpdateViusalScale();
        }
        protected abstract void OnTurnStart(CombatEntityActionCallback callback);
        protected abstract void OnTurnEnd(CombatEntityActionCallback callback);
        protected abstract void OnEnterTerrain(CombatEntityMapNodeInteractionCallback callback);
        protected abstract void OnExitTerrain(CombatEntityMapNodeInteractionCallback callback);

        private void Update()
        {

            UpdateViusalScale();
        }

        private void UpdateViusalScale()
        {
            if (parentNode != null)
            {
                if (scaleMemory != parentNode.transform.localScale.y)
                {
                    modifierVisual.transform.localScale = new Vector3(1f, 1f / parentNode.transform.localScale.y, 1f);
                    scaleMemory = parentNode.transform.localScale.y;
                }
            }
        }
    }
}
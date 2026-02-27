
using System;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using UnityEngine;
namespace TwilightAndBlight
{
    [RequireComponent(typeof(CombatEntity))]
    public class SpawnEntitiesOnDeath : MonoBehaviour
    {
        private CombatEntity combatEntity;
        [SerializeField] private bool relativeToSource;
        [SerializeField] private List<SpawnEntityOnDeathPrefabData> prefabs = new List<SpawnEntityOnDeathPrefabData>();
        protected virtual void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();
        }
        protected virtual void OnEnable()
        {
            GameEvents.OnEntityKilled += OnEntityKilled;
        }
        protected virtual void OnDisable()
        {
            GameEvents.OnEntityKilled -= OnEntityKilled;

        }
        protected virtual void OnEntityKilled(CombatEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                foreach (SpawnEntityOnDeathPrefabData data in prefabs)
                {
                    Vector3Int offset = data.spawnOffset;
                    if (relativeToSource)
                    {
                        Vector3 directionVector = new Vector3(callback.source.transform.position.x - callback.target.transform.position.x, 0, callback.source.transform.position.z - callback.target.transform.position.z);
                        Vector3 forwardVector = new Vector3(callback.source.transform.forward.x, 0, callback.source.transform.forward.z);
                        float angle = Vector3.Angle(forwardVector, directionVector);
                        int direction = Mathf.RoundToInt(angle / 60f);
                        for (int i = 0; i < direction; i++)
                        {
                            offset = new Vector3Int(-offset.z, offset.x, offset.y);
                        }
                    }
                    Vector2Int position = MapManager.Instance.GetRealativePosition(callback.target.GetCurrentMapNode().PositionInMap, offset);
                }
            }
        }

        protected virtual void SpawnEntity(GameObject prefab, Vector2Int position)
        {
            if (MapManager.Instance.GetMap().ContainsKey(position))
            {
                GameObject newEntityObj = Instantiate(prefab);
                CombatEntity newEntity = newEntityObj.GetComponent<CombatEntity>();
                MapNode targetNode = MapManager.Instance.GetMap()[position];
                if (!targetNode.IsOccupied())
                {
                    targetNode.AssignEntity(newEntity);
                }
            }
        }
    }

    
}

using System;
using UnityEngine;
namespace TwilightAndBlight
{
    [Serializable]
    public class SpawnEntityOnDeathPrefabData
    {
        [SerializeField] public GameObject prefab;
        [SerializeField] public Vector3Int spawnOffset;
    }
}

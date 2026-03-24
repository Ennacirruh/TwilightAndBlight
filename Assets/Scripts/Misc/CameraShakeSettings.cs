using System;
using System.Collections.Generic;
using TwilightAndBlight;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Map;
using UnityEngine;
[Serializable]
public class CameraShakeSettings
{
    [SerializeField] public Vector3 defaultCameraShakeDirection;
    [SerializeField] public float cameraShakeForce;
    [SerializeField] public List<VariableStatScaler> cameraShakeForceScalers = new List<VariableStatScaler>();
    [SerializeField] public CameraShakeMode cameraShakeMode;

    public Vector3 GetCameraShakeDirection(Vector3 source, Vector3 target)
    {
        Vector3 dir = Vector3.zero;
        Vector3 right = Vector3.zero;
        switch (cameraShakeMode)
        {
            case CameraShakeMode.CustomDirection:
                return defaultCameraShakeDirection.normalized;
            case CameraShakeMode.FromCameraToSource:
                return (source - Camera.main.transform.position).normalized;
            case CameraShakeMode.FromCameraToTarget:
                return (target - Camera.main.transform.position).normalized;
            case CameraShakeMode.FromSourceToTarget:
                return (target - source).normalized;
            case CameraShakeMode.PerpendicularToSourceAndTarget:
                dir = (target - source);
                right = Vector3.Cross(Vector3.up, dir).normalized;
                return right.normalized;
            case CameraShakeMode.PerpendicularToSourceAndCamera:
                dir = (source - Camera.main.transform.position);
                right = Vector3.Cross(Vector3.up, dir).normalized;
                return right.normalized;
            case CameraShakeMode.PerpendicularToTargetAndCamera:
                dir = (target - Camera.main.transform.position);
                right = Vector3.Cross(Vector3.up, dir).normalized;
                return right.normalized;
            case CameraShakeMode.RandomDirection:
                return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            default:
                return defaultCameraShakeDirection.normalized;
        }
    }
}

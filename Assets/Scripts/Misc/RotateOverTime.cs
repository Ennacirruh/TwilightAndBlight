using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private Space space = Space.Self;
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, space);

    }
}

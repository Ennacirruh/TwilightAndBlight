using UnityEngine;

public class RandomizeColorMono : MonoBehaviour
{
    [SerializeField] private Vector2 colorRange;
    private Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
        float color = Random.Range(colorRange.x, colorRange.y);
        Color newColor = new Color(color, color, color, 1);
        material.color = newColor;  
    }


}

using System.Collections;
using UnityEngine;

public class ResourceBarController : MonoBehaviour
{
    [SerializeField] RectTransform resourceBar;
    [SerializeField] RectTransform resourceShadowBar;
    [SerializeField] private bool xShift;
    [SerializeField] private bool yShift;
    private float xMax;
    private float yMax;
    private float shadowTarget;
    private float shadowT;
    private const float shadowSpeed = .5f;
    private Coroutine shadowBarCoroutine;
    private void Awake()
    {
        xMax = resourceBar.localScale.x;
        yMax = resourceBar.localScale.y;
        shadowT = 1f;
    }

    public void SetBarProgress(float t)
    {
        t = Mathf.Clamp01(t);
        shadowTarget = t;
        resourceBar.localScale = GetNewScale(t);
        if(shadowBarCoroutine == null)
        {
            shadowBarCoroutine = StartCoroutine(ShadowBarCoroutine());
        }
    }

    private IEnumerator ShadowBarCoroutine()
    {
        while(shadowT != shadowTarget)
        {
            shadowT = Mathf.MoveTowards(shadowT, shadowTarget, shadowSpeed * Time.deltaTime);
            resourceShadowBar.localScale = GetNewScale(shadowT);
            yield return null;
        }
        shadowBarCoroutine = null;
    }
    private Vector2 GetNewScale(float t)
    {
       
        float x = xMax;
        float y = yMax;
        if (xShift)
        {
            x = xMax * t;
        }
        if (yShift)
        {
            y = yMax * t;
        }
        return new Vector2(x, y);
    }
}

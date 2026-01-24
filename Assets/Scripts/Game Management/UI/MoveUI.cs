using System.Data.Common;
using TwilightAndBlight;
using UnityEngine;
using UnityEngine.EventSystems;
public class MoveUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private RectTransform targetTransform;
    private RectTransform canvasRect;
    
    [SerializeField] private Vector2 origin;
    private Vector2 mouseOrigin;

    private void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if(canvas != null)
        {
            canvasRect = canvas.gameObject.GetComponent<RectTransform>();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 cameraScale = new Vector2(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);
        Vector2 delta = eventData.position - mouseOrigin;
        delta = delta / cameraScale * canvasRect.sizeDelta;
        //Debug.Log($"Origin: {origin}, Delta: {delta}, DeltaToScreen: {GameUtility.ScreenToCanvasPosition(canvasRect, delta)}");
        targetTransform.anchoredPosition = origin + delta;
        
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        mouseOrigin = eventData.position;
        origin = targetTransform.anchoredPosition;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

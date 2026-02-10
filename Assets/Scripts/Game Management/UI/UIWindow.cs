using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIWindow : MonoBehaviour
{
   
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject expandapleParent;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private ScrollRect scrollRect;
    private List<GameObject> content = new List<GameObject>();
    public Vector2 WindowSize { get{ return rectTransform.sizeDelta; } }


    public void ToggleContent()
    {
        expandapleParent.SetActive(!expandapleParent.activeSelf);
    }
    public void SetWindowSize(Vector2 windowSize)
    {
        rectTransform.sizeDelta = windowSize;
    }
    public void SetWindowPosition(Vector2 windowPosition)
    {
        rectTransform.anchoredPosition = windowPosition;
    }
    public void AssignContent(GameObject content)
    {
        GameObject newContent = Instantiate(content);
        newContent.GetComponent<RectTransform>().SetParent(contentParent, false);
        this.content.Add( newContent);
        
        //scrollRect.content = newContent.GetComponent<RectTransform>();
        //scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.sizeDelta.x / 2f ,0);
    }
    public GameObject GetContent(int index)
    {
        return content[index];
    }
}

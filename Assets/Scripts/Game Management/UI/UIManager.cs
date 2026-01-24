using System.Collections.Generic;
using TwilightAndBlight;
using TwilightAndBlight.Ability;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private GameObject AbilityListPrefab;
    [SerializeField] private GameObject AbilityPreviewPrefab;
    private Dictionary<string, UIWindow> windowCache = new Dictionary<string, UIWindow>();
    private List<AbilityPreview> abilityPreviewPool = new List<AbilityPreview>();
    private List<AbilityPreview> inUse = new List<AbilityPreview>();
    private UIWindow abilityPreview;
    private bool previewWindowOpen;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public UIWindow OpenNewUIWindow(Vector2 pos, Vector2 size, string cacheID = "")
    {

        UIWindow window;
        if (cacheID != "" && windowCache.ContainsKey(cacheID))
        {
            window = windowCache[cacheID];
            window.gameObject.SetActive(true);
        }
        else
        {
            GameObject windowObj = Instantiate(windowPrefab, canvas);
            window = windowObj.GetComponent<UIWindow>();
            if(cacheID != "")
            {
                windowCache.Add(cacheID, window);
            }
        }    
        window.SetWindowSize(size);
        window.SetWindowPosition(pos);
        return window;
    }
    public void CloseUIWindow(UIWindow window)
    {
        if (windowCache.ContainsValue(window))
        {
            window.gameObject.SetActive(false);
        }
        else
        {
            Destroy(window.gameObject);
        }
    }
    private UIWindow OpenNewAbilityViewWindow()
    {
        bool exists = windowCache.ContainsKey("AbilityView");
        UIWindow window = OpenNewUIWindow(new Vector2(700, 0), new Vector2(520, 1080), "AbilityView");
        if (!exists)
        {
            window.AssignContent(AbilityListPrefab);
        }
        return window;
    }
   
    public void PreviewAbilities(CombatEntity entity)
    {
        UIWindow window = OpenNewAbilityViewWindow();
        GameObject content = window.GetContent();
        RectTransform contentTransform = content.GetComponent<RectTransform>();
        foreach(EntityAbility ability in entity.GetEntityAbilities())
        {
            AbilityPreview preview;
            if(abilityPreviewPool.Count > 0)
            {
                preview = abilityPreviewPool[abilityPreviewPool.Count - 1];
                abilityPreviewPool.RemoveAt(abilityPreviewPool.Count - 1);
                preview.transform.SetParent(contentTransform, false);
                preview.gameObject.SetActive(true);
            }
            else
            {
                GameObject previewObj = Instantiate(AbilityPreviewPrefab, contentTransform, worldPositionStays: false);
                preview = previewObj.GetComponent<AbilityPreview>();
            }
            preview.DisplayAbility(ability, entity);
            inUse.Add(preview);
            abilityPreview = window;
            previewWindowOpen = true;
        }
    }
    public void CloseAbilityPreview()
    {
        if (previewWindowOpen)
        {
            previewWindowOpen = false;
            AbilityPreview[] previews = abilityPreview.GetContent().GetComponentsInChildren<AbilityPreview>();
            foreach (AbilityPreview preview in previews)
            {
                abilityPreviewPool.Add(preview);
                inUse.Remove(preview);
                preview.gameObject.SetActive(false);
                preview.transform.SetParent(canvas, false);
                preview.CloseAbilityDescription();

            }
            abilityPreview.gameObject.SetActive(false);
        }
    }
    public RectTransform GetCanvas()
    {
        return canvas;
    }




}

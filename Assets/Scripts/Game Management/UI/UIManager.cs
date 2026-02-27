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
    [SerializeField] private GameObject StatPreviewListPrefab;
    [SerializeField] private GameObject DescriptionViewPrefab;
    [SerializeField] private GameObject VictoryScreen;
    [SerializeField] private GameObject DefeatScreen;
    private Dictionary<string, UIWindow> windowCache = new Dictionary<string, UIWindow>();
    private List<AbilityPreview> abilityPreviewPool = new List<AbilityPreview>();
    private List<AbilityPreview> inUse = new List<AbilityPreview>();
    private UIWindow abilityPreview;
    private UIWindow statPreview;
    private GameObject victoryScreenInstance;
    private GameObject defeatScreenInstance;
    private bool abilityPreviewWindowOpen;
    private bool statPreviewWindowOpen;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            victoryScreenInstance = Instantiate(VictoryScreen, canvas);
            victoryScreenInstance.SetActive(false);
            defeatScreenInstance = Instantiate(DefeatScreen, canvas);
            defeatScreenInstance.SetActive(false);
        }
        else
        {
            Destroy(this);
        }
    }
    public void OpenVictoryScreen()
    {
        victoryScreenInstance.SetActive(true);
    }
    public void OpenDefeatScreen()
    {
        defeatScreenInstance.SetActive(true);
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
        UIWindow window = OpenNewUIWindow(new Vector2(700, 0), new Vector2(520, 770), "AbilityView");
        if (!exists)
        {
            window.AssignContent(DescriptionViewPrefab);
            window.AssignContent(AbilityListPrefab);
        }
        return window;
    }
    private UIWindow OpenNewStatViewWindow()
    {
        bool exists = windowCache.ContainsKey("StatView");
        UIWindow window = OpenNewUIWindow(new Vector2(-810, 0), new Vector2(300, 990), "StatView");
        if (!exists)
        {
            window.AssignContent(DescriptionViewPrefab);
            window.AssignContent(StatPreviewListPrefab);
        }
        return window;
    }
    public void PreviewStats(CombatEntity entity)
    {
        UIWindow window = OpenNewStatViewWindow();
        GameObject description = window.GetContent(0);
        GameObject statView = window.GetContent(1);
        DescriptionView descriptionView = description.GetComponentInChildren<DescriptionView>();
        StatPreviewManager previewManager = statView.GetComponentInChildren<StatPreviewManager>();
        previewManager.InitializeStatManager(descriptionView, entity);
        statPreview = window;
        statPreviewWindowOpen = true;
         
    }
    public void PreviewAbilities(CombatEntity entity)
    {
        UIWindow window = OpenNewAbilityViewWindow();
        GameObject description = window.GetContent(0);
        GameObject abilityList = window.GetContent(1);
        RectTransform contentTransform = abilityList.GetComponent<RectTransform>();
        DescriptionView descriptionView = description.GetComponentInChildren<DescriptionView>();
        descriptionView.AssignPrefix("Passive: ");
        descriptionView.AssignDefaultDescriptable(entity.GetEntityPassive());
        descriptionView.PreviewDefault();
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
            preview.DisplayAbility(ability, entity, descriptionView);
            inUse.Add(preview);
            abilityPreview = window;
            abilityPreviewWindowOpen = true;
        }
    }
    public void CloseAbilityPreview()
    {
        if (abilityPreviewWindowOpen)
        {
            abilityPreviewWindowOpen = false;
            AbilityPreview[] previews = abilityPreview.GetContent(1).GetComponentsInChildren<AbilityPreview>();
            foreach (AbilityPreview preview in previews)
            {
                abilityPreviewPool.Add(preview);
                inUse.Remove(preview);
                preview.gameObject.SetActive(false);
                preview.transform.SetParent(canvas, false);

            }
            abilityPreview.gameObject.SetActive(false);
        }
    }
    public void CloseStatPreview()
    {
        if (statPreviewWindowOpen)
        {
            statPreview.gameObject.SetActive(false);
        }
    }
    public RectTransform GetCanvas()
    {
        return canvas;
    }




}

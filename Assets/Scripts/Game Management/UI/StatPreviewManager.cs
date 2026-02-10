using System;
using System.Collections.Generic;
using TwilightAndBlight;
using UnityEngine;

public class StatPreviewManager : MonoBehaviour
{
    [SerializeField] private EntityStats stats;
    [SerializeField] private GameObject statPreviewPrefab;
    [SerializeField] private Dictionary<StatType, StatPreview> statPreviewDict = new Dictionary<StatType, StatPreview>();
    public void InitializeStatManager(DescriptionView descriptionView, CombatEntity entity)
    {

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            StatPreview statPreview;
            if (!statPreviewDict.ContainsKey(statType))
            {
                GameObject previewer = Instantiate(statPreviewPrefab, transform);
                statPreview = previewer.GetComponent<StatPreview>();
                statPreviewDict.Add(statType, statPreview);
            }
            else
            {
                statPreview = statPreviewDict[statType];
            }
            stats = entity.Stats;
            statPreview.DistplayStat(stats, statType, descriptionView);
        }
    }
    
}

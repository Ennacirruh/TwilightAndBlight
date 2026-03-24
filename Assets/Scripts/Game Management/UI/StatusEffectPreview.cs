using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectPreview : MonoBehaviour
{
    private Image statusEffectIcon;
    private TextMeshProUGUI stackCounter;

    public Image StatusEffectIcon { get { return statusEffectIcon; } }
    public TextMeshProUGUI StackCounter { get { return stackCounter; } }

    private void Awake()
    {
        statusEffectIcon = GetComponent<Image>();
        stackCounter = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayNewStacks(int stackCount)
    {
        StackCounter.text = (stackCount > 0) ? stackCount.ToString() : string.Empty;
    }
}

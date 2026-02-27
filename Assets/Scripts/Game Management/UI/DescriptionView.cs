using TMPro;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Ability.Passive;
using UnityEngine;
namespace TwilightAndBlight 
{
    public class DescriptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private IDescriptable defaultDisplay;
        [SerializeField] private string prefix;
        public void AssignPrefix(string prefix)
        {
            this.prefix = prefix;
        }
        public void AssignDefaultDescriptable(IDescriptable descriptable)
        {
            defaultDisplay = descriptable; 
        }
        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }
        public void PreviewDescriptable(IDescriptable descriptable, bool usePrefix = true)
        {
            
            if (descriptable != null)
            {
                string actualPrefix = "";
                if (usePrefix) { actualPrefix = prefix; }
                descriptionText.text = actualPrefix + descriptable.GetName() + "\n" + descriptable.GetDescription();
            }
            else
            {
                descriptionText.text = "";
            }
        }
        public void PreviewDefault()
        {
            PreviewDescriptable(defaultDisplay);
            
        }
    }
}

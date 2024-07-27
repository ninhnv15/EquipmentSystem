namespace UIFeatures.Inventory
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Model;

    public class ProgressBarStatItemView : BaseStatItemView
    {
        [field: SerializeField] public Slider          ProgressBar { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtValue    { get; private set; }


        public override void SetStatValue(StatDataElement statDataElement)
        {
            var clampedStat = statDataElement.ClampedBy ?? statDataElement;
            this.ProgressBar.maxValue = clampedStat.CurrentValue;
            this.ProgressBar.value    = statDataElement.CurrentValue;
            this.TxtValue.text        = $"{statDataElement.CurrentValue}/{clampedStat.CurrentValue}";
        }
        public override void SetStatColor(Color color) { this.ProgressBar.fillRect.GetComponent<Image>().color = color; }
    }
}
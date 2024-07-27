namespace UIFeatures.Inventory
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Model;

    public class ProgressBarStatItemView : BaseStatItemView
    {
        [field: SerializeField] public Image           ImgFill           { get; private set; }
        [field: SerializeField] public Image           ImgFillChange     { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtValue          { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtMaxValueChange { get; private set; }

        private float currentValue;
        private float currentMaxValue;

        public override void SetStatValue(StatDataElement statDataElement)
        {
            var clampedStat = statDataElement.ClampedBy ?? statDataElement;

            this.currentValue    = statDataElement.CurrentValue;
            this.currentMaxValue = clampedStat.CurrentValue;

            this.ImgFill.fillAmount = (float)this.currentValue / this.currentMaxValue;
            this.TxtValue.text      = $"{this.currentValue}/{this.currentMaxValue}";
        }
        public override void SetStatColor(Color color)         { this.ImgFill.color            = color; }
        public override void SetChangeValue(float changeValue) { this.ImgFillChange.fillAmount = changeValue == 0 ? 0 : (this.currentValue + changeValue) / this.currentMaxValue; }

        public override void SetChangeMaxValue(float changeMaxValue)
        {
            if (changeMaxValue == 0)
            {
                this.TxtMaxValueChange.gameObject.SetActive(false);
                return;
            }

            this.TxtMaxValueChange.gameObject.SetActive(true);
            this.TxtMaxValueChange.text = $"{(changeMaxValue < 0 ? "-" : "+")} {changeMaxValue}";
        }
    }
}
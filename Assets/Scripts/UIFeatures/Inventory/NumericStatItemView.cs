namespace UIFeatures.Inventory
{
    using System.Globalization;
    using TMPro;
    using UnityEngine;
    using UserData.Model;

    public class NumericStatItemView : BaseStatItemView
    {
        [field: SerializeField] public TextMeshProUGUI TxtBaseValue        { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAddedValue       { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAddedValueChange { get; private set; }

        public override void SetStatValue(StatDataElement statDataElement)
        {
            this.TxtBaseValue.text  = statDataElement.BaseValue.ToString(CultureInfo.InvariantCulture);
            this.TxtAddedValue.text = $"+ {statDataElement.AddedValue}";
        }
        public override void SetStatColor(Color color) { }
        public override void SetChangeValue(float changeValue)
        {
            if (changeValue == 0)
            {
                this.TxtAddedValueChange.gameObject.SetActive(false);
                return;
            }

            this.TxtAddedValueChange.gameObject.SetActive(true);
            this.TxtAddedValueChange.text = $"{(changeValue < 0 ? "-" : "+")} {changeValue}";
        }
    }
}
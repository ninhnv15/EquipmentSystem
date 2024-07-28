namespace UIFeatures.Inventory
{
    using System;
    using System.Globalization;
    using DG.Tweening;
    using GameFoundation.Scripts.Utilities.Extension;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UserData.Model;

    public class NumericStatItemView : BaseDetailStatItemView
    {
        [field: SerializeField] public TextMeshProUGUI TxtBaseValue        { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAddedValue       { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAddedValueChange { get; private set; }

        public override void SetStatValue(StatDataElement statDataElement)
        {
            this.SubscribeToText(statDataElement.BaseValue, this.TxtBaseValue);
            this.SubscribeToText(statDataElement.AddedValue, this.TxtAddedValue);
        }

        private void SubscribeToText(IObservable<float> source, TextMeshProUGUI text)
        {
            var isFirstTime = true;
            source.Subscribe(value =>
            {
                if (isFirstTime)
                {
                    text.text   = value.ToString("0.00");
                    isFirstTime = false;
                    return;
                }

                DOTween.To(() => float.Parse(text.text), x => text.text = x.ToString("0.00"), value, 0.5f).SetEase(Ease.OutQuart);
            }).AddTo(this);
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
            this.TxtAddedValueChange.text = changeValue > 0 ? $"+ {changeValue}" : changeValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
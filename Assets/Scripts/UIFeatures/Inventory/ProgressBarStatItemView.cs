namespace UIFeatures.Inventory
{
    using System;
    using System.Globalization;
    using DG.Tweening;
    using TMPro;
    using UniRx;
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
            this.SubscribeValue(clampedStat, () => this.currentMaxValue, value => this.currentMaxValue = value);
            this.SubscribeValue(statDataElement, () => this.currentValue, value => this.currentValue   = value);
        }

        private void SubscribeValue(StatDataElement statDataElement, Func<float> getValue, Action<float> setValue)
        {
            var isFirstTime = true;
            statDataElement.CurrentValue.Subscribe(value =>
            {
                if (!isFirstTime)
                {
                    DOTween.To(() => (int)getValue(), x =>
                    {
                        setValue(x);
                        this.RefreshValueView();
                    }, (int)value, 0.5f).SetEase(Ease.OutQuart);
                }
                else
                {
                    setValue(value);
                    this.RefreshValueView();
                    isFirstTime = false;
                }
            }).AddTo(this);
        }


        private void RefreshValueView()
        {
            this.ImgFill.fillAmount = (float)this.currentValue / this.currentMaxValue;
            this.TxtValue.text      = $"{this.currentValue:0}/{this.currentMaxValue:0}";
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
            this.TxtMaxValueChange.text = changeMaxValue > 0 ? $"+ {changeMaxValue}" : changeMaxValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
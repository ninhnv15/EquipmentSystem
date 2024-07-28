namespace UserData.Model
{
    using System;
    using System.Collections.Generic;
    using Blueprints;

    public class StatContainer
    {
        private Dictionary<StatType, StatDataElement> statsDictionary = new();

        public Action<StatDataElement> OnStatChanged;

        public void AddStatData(StatType statType, float value)
        {
            this.statsDictionary[statType] = new StatDataElement(statType, value);
            this.OnStatChange(this.statsDictionary[statType]);
        }

        public bool HasStat(StatType statType) { return this.statsDictionary.ContainsKey(statType); }

        public StatDataElement GetStat(StatType statType) { return this.statsDictionary.GetValueOrDefault(statType); }

        public float GetStatValue(StatType statType) { return this.statsDictionary[statType].CurrentValue.Value; }

        public void SetBaseStatValue(StatType statType, float value)
        {
            this.statsDictionary[statType].SetBaseValue(value);
            this.OnStatChange(this.statsDictionary[statType]);
        }

        public void AddAddedStatValue(StatType statType, float value)
        {
            var statDataElement = this.statsDictionary[statType];
            statDataElement.SetAddedValue(statDataElement.AddedValue.Value + value);
            this.OnStatChange(statDataElement);
        }

        #region Handle stat change

        protected virtual void OnStatChange(StatDataElement statDataElement) { this.OnStatChanged?.Invoke(statDataElement); }

        #endregion
    }
}
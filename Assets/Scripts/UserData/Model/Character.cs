namespace UserData.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints;
    using Newtonsoft.Json;
    using UniRx;

    public class Character
    {
        public string Id { get; }

        public Dictionary<StatType, StatDataElement> StatsDictionary { get; }

        public Dictionary<ItemType, SlotItem> SlotItems { get; } = new();

        [JsonIgnore] public CharacterRecord StaticData { get; set; }

        public Character(string id, Dictionary<StatType, StatDataElement> statsDictionary)
        {
            this.Id              = id;
            this.StatsDictionary = statsDictionary;
        }

        public bool EquipItem(Item newItem, out Item oldItem)
        {
            oldItem = null;
            if (newItem.StaticData.ItemType != ItemType.Consumable)
            {
                if (!this.SlotItems.TryGetValue(newItem.StaticData.ItemType, out var slotItem))
                {
                    slotItem = this.AddSlotItem(newItem.StaticData.ItemType);
                }

                if (slotItem.Item.Value != null)
                {
                    oldItem = slotItem.Item.Value;
                    foreach (var stat in oldItem.StaticData.StatToValue)
                    {
                        var statDataElement = this.StatsDictionary[stat.Key];
                        statDataElement.SetAddedValue(statDataElement.AddedValue.Value - stat.Value);
                    }
                }

                slotItem.Item.Value = newItem;
            }
            else
            {
                // return false if all stats are max value
                if (newItem.StaticData.StatToValue.All(stat => this.StatsDictionary[stat.Key].IsMaxValue())) return false;
            }

            foreach (var stat in newItem.StaticData.StatToValue)
            {
                var statDataElement = this.StatsDictionary[stat.Key];
                statDataElement.SetAddedValue(statDataElement.AddedValue.Value + stat.Value);
            }

            return true;
        }

        public SlotItem AddSlotItem(ItemType itemType)
        {
            var slotItem = new SlotItem(itemType);
            this.SlotItems.Add(itemType, slotItem);
            return slotItem;
        }

        public SlotItem GetOrAddSlotItem(ItemType itemType) { return this.SlotItems.TryGetValue(itemType, out var slotItem) ? slotItem : this.AddSlotItem(itemType); }
    }

    public class StatDataElement
    {
        public StatType              Type         { get; set; }
        public FloatReactiveProperty BaseValue    { get; set; }
        public FloatReactiveProperty AddedValue   { get; set; }
        public FloatReactiveProperty CurrentValue { get; set; }

        [JsonIgnore] public StatRecord      StatRecord { get; set; }
        [JsonIgnore] public StatDataElement ClampedBy  { get; set; }
        public StatDataElement(StatType statName, float originValue)
        {
            this.Type         = statName;
            this.BaseValue    = new FloatReactiveProperty(originValue);
            this.AddedValue   = new FloatReactiveProperty(0);
            this.CurrentValue = new FloatReactiveProperty(originValue);
        }

        public void SetAddedValue(float newAddedValue)
        {
            if (this.ClampedBy != null)
            {
                var clampedValue = this.ClampedBy.CurrentValue.Value;
                if (this.BaseValue.Value + newAddedValue > clampedValue)
                {
                    newAddedValue = clampedValue - this.BaseValue.Value;
                }
            }

            this.AddedValue.Value = newAddedValue;
            this.UpdateCurrentValue();
        }

        public void SetBaseValue(float newBaseValue)
        {
            if (this.ClampedBy != null)
            {
                var clampedValue = this.ClampedBy.CurrentValue.Value;
                if (newBaseValue + this.AddedValue.Value > clampedValue)
                {
                    newBaseValue = clampedValue - this.AddedValue.Value;
                }
            }

            this.BaseValue.Value = newBaseValue;
            this.UpdateCurrentValue();
        }

        public bool IsMaxValue()
        {
            if (this.ClampedBy == null) return false;
            return this.CurrentValue.Value >= this.ClampedBy.CurrentValue.Value;
        }

        private void UpdateCurrentValue() { this.CurrentValue.Value = this.BaseValue.Value + this.AddedValue.Value; }
    }

    public class SlotItem
    {
        public ItemType               ItemType;
        public ReactiveProperty<Item> Item;

        public SlotItem(ItemType itemType)
        {
            this.ItemType = itemType;
            this.Item     = new ReactiveProperty<Item>();
        }
    }
}
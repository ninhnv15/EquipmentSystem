namespace UserData.Model
{
    using System.Collections.Generic;
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
            if (newItem.StaticData.ItemType == ItemType.Consumable) return false;
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

        public void SetAddedValue(float value)
        {
            this.AddedValue.Value = value;
            this.UpdateCurrentValue();
        }

        public void SetBaseValue(float value)
        {
            this.BaseValue.Value = value;
            this.UpdateCurrentValue();
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
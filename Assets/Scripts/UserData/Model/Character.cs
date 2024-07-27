namespace UserData.Model
{
    using System.Collections.Generic;
    using Blueprints;
    using Newtonsoft.Json;

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

        public bool EquipItem(Item item)
        {
            if (item.StaticData.ItemType == ItemType.Consumable) return false;
            if (!this.SlotItems.TryGetValue(item.StaticData.ItemType, out var slotItem))
            {
                slotItem = new SlotItem { ItemType = item.StaticData.ItemType };
                this.SlotItems.Add(item.StaticData.ItemType, slotItem);
            }

            slotItem.Item = item;
            return true;
        }
    }

    public class StatDataElement
    {
        public StatType Type         { get; set; }
        public float    BaseValue    { get; set; }
        public float    AddedValue   { get; set; }
        public float    CurrentValue => this.BaseValue + this.AddedValue;

        [JsonIgnore] public StatRecord      StatRecord { get; set; }
        [JsonIgnore] public StatDataElement ClampedBy  { get; set; }
        public StatDataElement(StatType statName, float originValue)
        {
            this.Type       = statName;
            this.BaseValue  = originValue;
            this.AddedValue = 0;
        }

        public void SetAddedValue(float value) { this.AddedValue = value; }

        public void SetBaseValue(float value) { this.BaseValue = value; }
    }

    public class SlotItem
    {
        public ItemType ItemType;
        public Item     Item;
    }
}
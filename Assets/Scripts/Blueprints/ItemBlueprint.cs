namespace Blueprints
{
    using System;
    using System.Collections.Generic;
    using DataManager.Blueprint.BlueprintReader;

    [BlueprintReader("Items")]
    public class ItemBlueprint : GenericBlueprintReaderByRow<string, ItemRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class ItemRecord
    {
        public string   Id;
        public string   Name;
        public string   Description;
        public string   Icon;
        public ItemType ItemType;

        public Dictionary<StatType, float> StatToValue;
        
        public int DefaultAmount;
    }
    

    public enum ItemType
    {
        Weapon,
        Armor,
        Helmet,
        Shield,
        Boots,
        Gloves,
        Consumable,
    }
}
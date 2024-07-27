namespace Blueprints
{
    using DataManager.Blueprint.BlueprintReader;

    [BlueprintReader("Stats")]
    public class StatBlueprint : GenericBlueprintReaderByRow<StatType, StatRecord>
    {
    }

    [CsvHeaderKey("Type")]
    public class StatRecord
    {
        public StatType Type;
        public string   Name;
        public string   Icon;
        public string   Description;
        
        public StatType ClampedBy;
        
        public bool   IsShow;
        public string StatItemViewPath;
        public string ColorHex;
    }

    public enum StatType
    {
        None,
        Health,
        Mana,
        Strength,
        Agility,
        Intelligence,
        MaxHealth,
        MaxMana
    }
}
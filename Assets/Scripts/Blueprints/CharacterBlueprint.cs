namespace Blueprints
{
    using System.Collections.Generic;
    using DataManager.Blueprint.BlueprintReader;

    [BlueprintReader("Characters")]
    public class CharacterBlueprint : GenericBlueprintReaderByRow<string, CharacterRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class CharacterRecord
    {
        public string Id;
        public string Name;
        public string Description;
        public string Avatar;
        public string ModelAssetPath;
        public bool   IsDefault;

        public float BaseHealth;
        public float MaxHealth;
        public float BaseMana;
        public float MaxMana;
        public float BaseStrength;
        public float BaseAgility;
        public float BaseIntelligence;


        public Dictionary<StatType, float> GetBaseStats() =>
            new()
            {
                { StatType.Health, this.BaseHealth },
                { StatType.MaxHealth, MaxHealth },
                { StatType.Mana, this.BaseMana },
                { StatType.MaxMana, MaxMana },
                { StatType.Strength, this.BaseStrength },
                { StatType.Agility, this.BaseAgility },
                { StatType.Intelligence, this.BaseIntelligence }
            };
    }
}
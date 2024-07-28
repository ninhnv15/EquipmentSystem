namespace UserData.Controller
{
    using System.Collections.Generic;
    using Blueprints;
    using DataManager.MasterData;
    using DataManager.UserData;
    using UserData.Model;

    public class CharacterManager : BaseDataManager<UserProfile>
    {
        #region Inject

        private readonly CharacterBlueprint charactersBlueprint;
        private readonly StatBlueprint      statBlueprint;
        private readonly ItemBlueprint      itemBlueprint;
        private readonly InventoryManager   inventoryManager;

        public CharacterManager(MasterDataManager masterDataManager, CharacterBlueprint charactersBlueprint, StatBlueprint statBlueprint, ItemBlueprint itemBlueprint,
            InventoryManager inventoryManager) : base(masterDataManager)
        {
            this.charactersBlueprint = charactersBlueprint;
            this.statBlueprint       = statBlueprint;
            this.itemBlueprint       = itemBlueprint;
            this.inventoryManager    = inventoryManager;
        }

        protected override void OnDataLoaded()
        {
            foreach (var characterRecord in this.charactersBlueprint.Values)
            {
                if (this.Data.CharactersData.TryGetValue(characterRecord.Id, out var character))
                {
                    character.StaticData = characterRecord;
                    foreach (var statDataElement in character.StatsDictionary)
                    {
                        statDataElement.Value.StatRecord = this.statBlueprint.GetDataById(statDataElement.Key);
                        if (statDataElement.Value.StatRecord.ClampedBy != StatType.None)
                            statDataElement.Value.ClampedBy = character.StatsDictionary[statDataElement.Value.StatRecord.ClampedBy];
                    }

                    foreach (var slotItem in character.SlotItems.Values)
                    {
                        if (slotItem.Item.Value != null)
                        {
                            slotItem.Item.Value.StaticData = this.itemBlueprint.GetDataById(slotItem.Item.Value.Id);
                        }
                    }
                }
                else
                {
                    if (!characterRecord.IsDefault) continue;
                    this.Data.CharactersData.Add(characterRecord.Id, this.CreateCharacter(characterRecord.Id));
                    this.Data.SelectedCharacterId = characterRecord.Id;
                }
            }
        }

        #endregion

        #region Character

        public bool IsCharacterUnlocked(string characterId) { return this.Data.CharactersData.ContainsKey(characterId); }
        public int  UnlockedCharacterCount                  => this.Data.CharactersData.Count;


        public Character GetCharacter(string characterId)
        {
            if (!this.IsCharacterUnlocked(characterId)) return null;
            var character = this.Data.CharactersData[characterId];
            return character;
        }


        public string GetSelectedCharacterId() { return this.Data.SelectedCharacterId; }

        public Character GetSelectedCharacter() { return this.GetCharacter(this.Data.SelectedCharacterId); }

        public Character CreateCharacter(string characterId)
        {
            var baseStats       = this.charactersBlueprint.GetDataById(characterId).GetBaseStats();
            var statsDictionary = new Dictionary<StatType, StatDataElement>();

            foreach (var baseStat in baseStats)
            {
                var statDataElement = new StatDataElement(baseStat.Key, baseStat.Value)
                {
                    StatRecord = this.statBlueprint.GetDataById(baseStat.Key)
                };
                statsDictionary.Add(baseStat.Key, statDataElement);
            }

            foreach (var statDataElement in statsDictionary)
            {
                if (statDataElement.Value.StatRecord.ClampedBy != StatType.None)
                    statDataElement.Value.ClampedBy = statsDictionary[statDataElement.Value.StatRecord.ClampedBy];
            }

            return new Character(characterId, statsDictionary) { StaticData = this.charactersBlueprint.GetDataById(characterId) };
        }

        public bool SelectCharacter(string characterId)
        {
            if (!this.IsCharacterUnlocked(characterId)) return false;
            this.Data.SelectedCharacterId = characterId;
            return true;
        }

        public CharacterRecord GetCharacterRecord(string characterId) { return this.charactersBlueprint.GetDataById(characterId); }

        #endregion

        public bool TryEquipItem(Item newItem)
        {
            var character = this.GetSelectedCharacter();
            if (character == null) return false;
            if (!character.EquipItem(newItem, out var oldItem)) return false;
            
            this.inventoryManager.RemoveItem(newItem);
            this.inventoryManager.AddItem(oldItem);
            return true;

        }
    }
}
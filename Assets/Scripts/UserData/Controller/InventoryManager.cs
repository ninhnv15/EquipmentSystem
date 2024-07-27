namespace UserData.Controller
{
    using System.Collections.Generic;
    using Blueprints;
    using DataManager.MasterData;
    using DataManager.UserData;
    using UserData.Model;

    public class InventoryManager : BaseDataManager<UserInventory>
    {
        private readonly ItemBlueprint    itemBlueprint;

        public InventoryManager(MasterDataManager masterDataManager, ItemBlueprint itemBlueprint) : base(masterDataManager)
        {
            this.itemBlueprint    = itemBlueprint;
        }

        protected override void OnDataLoaded()
        {
            if (!this.Data.IsLoadItemFromBlueprint)
            {
                foreach (var itemRecord in this.itemBlueprint.Values)
                {
                    if (itemRecord.DefaultAmount > 0)
                    {
                        this.Data.ItemIdToItems.Add(itemRecord.Id, new Item()
                        {
                            Id         = itemRecord.Id,
                            StaticData = itemRecord,
                            Amount     = itemRecord.DefaultAmount
                        });
                    }
                }
                
                this.Data.IsLoadItemFromBlueprint = true;
            }
        }
        
        public Dictionary<string, Item> GetInventoryItems()
        {
            return this.Data.ItemIdToItems;
        }
    }
}
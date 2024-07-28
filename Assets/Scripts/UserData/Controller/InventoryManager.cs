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

            //setup static data
            foreach (var item in this.Data.ItemIdToItems.Values)
            {
                item.StaticData = this.itemBlueprint[item.Id];
            }
        }
        
        public Dictionary<string, Item> GetInventoryItems()
        {
            return this.Data.ItemIdToItems;
        }
        public void RemoveItem(Item selectedItem)
        {
            if (selectedItem == null) return;
            if (this.Data.ItemIdToItems.TryGetValue(selectedItem.Id, out var item))
            {
                item.Amount -= 1;
                if (item.Amount <= 0)
                {
                    this.Data.ItemIdToItems.Remove(selectedItem.Id);
                }
            }
        }
        public void AddItem(Item oldItem)
        {
            if(oldItem == null) return;
            if (!this.Data.ItemIdToItems.TryAdd(oldItem.Id, oldItem))
            {
                this.Data.ItemIdToItems[oldItem.Id].Amount += 1;
            }
        }
    }
}
namespace UserData.Model
{
    using System.Collections.Generic;
    using DataManager.LocalData;
    using DataManager.UserData;

    public class UserInventory : IUserData, ILocalData
    {
        public bool IsLoadItemFromBlueprint { get; set; }

        public readonly Dictionary<string, Item> ItemIdToItems = new();
    }
}
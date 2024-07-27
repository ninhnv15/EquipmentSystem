namespace UserData.Model
{
    using System.Collections.Generic;
    using DataManager.LocalData;
    using DataManager.UserData;

    public  class UserProfile : IUserData, ILocalData
    {
        public string UserName { get; set; }
        public string Avatar   { get; set; }

        public          string                        SelectedCharacterId { get; set; }
        public readonly Dictionary<string, Character> CharactersData = new();

    }
}
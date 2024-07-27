namespace UserData.Model
{
    using Blueprints;
    using Newtonsoft.Json;

    public class Item
    {
        public              string     Id;
        [JsonIgnore] public ItemRecord StaticData;
        public              int        Amount { get; set; }
    }
}
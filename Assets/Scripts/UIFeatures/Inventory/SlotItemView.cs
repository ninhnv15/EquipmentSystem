namespace UIFeatures.Inventory
{
    using Blueprints;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Model;

    public class SlotItemView : TViewMono
    {
        [field: SerializeField] public ItemType ItemType     { get; private set; }
        [field: SerializeField] public Image    IconItem     { get; private set; }
        [field: SerializeField] public Image    IconItemType { get; private set; }
    }
    
    public class SlotItemPresenter : BaseUIItemPresenter<SlotItemView, SlotItem>
    {
        public SlotItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }
        
        public override void BindData(SlotItem data)
        {
            if (data != null)
            {
                this.View.IconItem.gameObject.SetActive(false);
                this.View.IconItemType.gameObject.SetActive(false);
                this.GetItemIcon(data.Item.StaticData.Icon).ContinueWith(sprite =>
                {
                    this.View.IconItem.sprite = sprite;
                    this.View.IconItem.gameObject.SetActive(true);
                });
            }
            else
            {
                this.View.IconItem.gameObject.SetActive(false);
                this.View.IconItemType.gameObject.SetActive(true);
            }
        }
        
        private async UniTask<Sprite> GetItemIcon(string iconItemPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(iconItemPath); }
    }
    

}
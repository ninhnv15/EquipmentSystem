namespace UIFeatures.Inventory
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UserData.Model;

    public class InventoryItemView : TViewMono
    {
        
    }

    public class InventoryItemPresenter : BaseUIItemPresenter<InventoryItemView, InventoryItemModel>
    {
        public InventoryItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }
        
        public override void BindData(InventoryItemModel param)
        {
            
        }
    }

    public class InventoryItemModel
    {
        public Item Item { get; set; }
    }
}
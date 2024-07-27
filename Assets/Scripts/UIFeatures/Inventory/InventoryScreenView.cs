namespace UIFeatures.Inventory
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UserData.Controller;
    using Zenject;

    public class InventoryScreenView : BaseView
    {
        public InventoryItemGridViewAdapter inventoryItemGridViewAdapter;
        public CharacterView                characterView;
    }

    [ScreenInfo(nameof(InventoryScreenView))]
    public class InventoryScreenPresenter : BaseScreenPresenter<InventoryScreenView>
    {
        private readonly DiContainer      diContainer;
        private readonly InventoryManager inventoryManager;
        private readonly CharacterManager characterManager;

        private CharacterPresenter characterPresenter;
        private InventoryItemModel currentSelectedItem;
        public InventoryScreenPresenter(SignalBus signalBus, DiContainer diContainer, InventoryManager inventoryManager, CharacterManager characterManager) : base(signalBus)
        {
            this.diContainer      = diContainer;
            this.inventoryManager = inventoryManager;
            this.characterManager = characterManager;
        }

        public override UniTask BindData()
        {
            //Setup inventory grid view
            var listItems               = this.inventoryManager.GetInventoryItems().Values.ToList();
            var listInventoryItemModels = new List<InventoryItemModel>();


            foreach (var item in listItems)
            {
                var itemModel = new InventoryItemModel(item);
                itemModel.OnSelected = () => { this.OnSelected(itemModel); };

                listInventoryItemModels.Add(itemModel);
            }

            this.View.inventoryItemGridViewAdapter.InitItemAdapter(listInventoryItemModels, this.diContainer);

            //Setup character view

            this.characterPresenter = this.diContainer.Instantiate<CharacterPresenter>();
            this.characterPresenter.SetView(this.View.characterView);
            this.characterPresenter.BindData(this.characterManager.GetSelectedCharacter());

            return UniTask.CompletedTask;
        }

        private void OnSelected(InventoryItemModel itemData)
        {
            this.characterPresenter.ResetChangeValue();
            
            if (this.currentSelectedItem == itemData)
            {
                this.currentSelectedItem.IsSelected.Value = false;
                this.currentSelectedItem                  = null;
                return;
            }

            if (this.currentSelectedItem != null)
            {
                this.currentSelectedItem.IsSelected.Value = false;
            }

            this.currentSelectedItem                  = itemData;
            this.currentSelectedItem.IsSelected.Value = true;
            this.characterPresenter.TryItem(this.currentSelectedItem.Item);
        }


        public override void Dispose()
        {
            base.Dispose();
            this.characterPresenter.Dispose();
        }
    }
}
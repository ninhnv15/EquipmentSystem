namespace UIFeatures.Inventory
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Controller;
    using UserData.Model;
    using Zenject;

    public class InventoryScreenView : BaseView
    {
        [field: SerializeField] public Button                       BtnBackground                { get; private set; }
        [field: SerializeField] public InventoryItemGridViewAdapter InventoryItemGridViewAdapter { get; private set; }
        [field: SerializeField] public CharacterView                CharacterView                { get; private set; }
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
            this.RefreshListItemView();

            //Setup character view

            this.characterPresenter = this.diContainer.Instantiate<CharacterPresenter>(new[] { this });
            this.characterPresenter.SetView(this.View.CharacterView);
            this.characterPresenter.BindData(this.characterManager.GetSelectedCharacter());

            this.View.BtnBackground.onClick.AddListener(this.UnSelectCurrentItem);

            return UniTask.CompletedTask;
        }
        public void RefreshListItemView()
        {
            var listInventoryItemModels = new List<InventoryItemModel>();

            foreach (var item in this.inventoryManager.GetInventoryItems().Values)
            {
                var itemModel = new InventoryItemModel(item);
                itemModel.OnSelected    = () => { this.OnSelected(itemModel); };
                itemModel.InventoryRoot = this.View.transform;
                listInventoryItemModels.Add(itemModel);
            }

            this.View.InventoryItemGridViewAdapter.InitItemAdapter(listInventoryItemModels, this.diContainer);
        }

        public Item GetSelectedItem() { return this.currentSelectedItem?.Item; }

        public void UnSelectCurrentItem()
        {
            if (this.currentSelectedItem == null) return;
            this.characterPresenter.ResetChangeValue();
            this.currentSelectedItem.IsSelected.Value = false;
            this.currentSelectedItem                  = null;
        }

        private void OnSelected(InventoryItemModel itemData)
        {
            if (this.currentSelectedItem == itemData) return;

            this.UnSelectCurrentItem();

            this.currentSelectedItem                  = itemData;
            this.currentSelectedItem.IsSelected.Value = true;
            this.characterPresenter.TryItem(this.currentSelectedItem.Item);
        }


        public override void Dispose()
        {
            base.Dispose();
            this.characterPresenter.Dispose();
            this.View.BtnBackground.onClick.RemoveAllListeners();
        }
    }
}
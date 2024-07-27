namespace UIFeatures.Inventory
{
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
        public InventoryScreenPresenter(SignalBus signalBus, DiContainer diContainer, InventoryManager inventoryManager, CharacterManager characterManager) : base(signalBus)
        {
            this.diContainer      = diContainer;
            this.inventoryManager = inventoryManager;
            this.characterManager = characterManager;
        }

        public override UniTask BindData()
        {
            //Setup inventory grid view
            var inventoryItems = this.inventoryManager.GetInventoryItems().Values.ToList();
            var listItemModels = inventoryItems.Select(item => new InventoryItemModel
            {
                Item = item
            }).ToList();
            this.View.inventoryItemGridViewAdapter.InitItemAdapter(listItemModels, this.diContainer);
            
            //Setup character view
            
            this.characterPresenter = this.diContainer.Instantiate<CharacterPresenter>();
            this.characterPresenter.SetView(this.View.characterView);
            this.characterPresenter.BindData(this.characterManager.GetSelectedCharacter());
            
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.characterPresenter.Dispose();
        }
    }
}
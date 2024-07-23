namespace UIFeatures.MainScene
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class MainScreenView : BaseView
    {
        [field: SerializeField] public Button BtnInventory { get; private set; }
    }

    [ScreenInfo(nameof(MainScreenView))]
    public class MainScreenPresenter : BaseScreenPresenter<MainScreenView>
    {
        private readonly IScreenManager screenManager;

        public MainScreenPresenter(SignalBus signalBus, IScreenManager screenManager) : base(signalBus) { this.screenManager = screenManager; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync();
            this.View.BtnInventory.onClick.AddListener(this.OnOpenInventory);
        }

        private void OnOpenInventory() { }

        public override UniTask BindData() { return UniTask.CompletedTask; }
    }
}
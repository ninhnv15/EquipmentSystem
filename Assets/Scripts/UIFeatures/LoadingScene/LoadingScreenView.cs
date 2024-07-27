namespace UIFeatures.LoadingScene
{
    using Cysharp.Threading.Tasks;
    using DataManager.MasterData;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundationBridge;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    [ScreenInfo(nameof(LoadingScreenView))]
    public class LoadingScreenPresenter : BaseScreenPresenter<LoadingScreenView>
    {
        private readonly GameSceneDirector sceneDirector;
        private readonly MasterDataManager masterDataManager;

        public LoadingScreenPresenter(SignalBus signalBus, GameSceneDirector sceneDirector, MasterDataManager masterDataManager) : base(signalBus)
        {
            this.sceneDirector     = sceneDirector;
            this.masterDataManager = masterDataManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();
        }

        public override UniTask BindData()
        {
            this.Init();

            return UniTask.CompletedTask;
        }

        private async void Init()
        {
            await (
                this.FakeLoading(),
                this.masterDataManager.InitializeData()
            );

            this.sceneDirector.LoadMainScene();
        }

        private async UniTask FakeLoading()
        {
            await DOTween.To(
                getter: () => 0f,
                setter: percent => this.View.SldLoading.value = percent / 100f,
                endValue: 100f,
                duration: 1f
            ).SetEase(Ease.Linear).AsyncWaitForCompletion();
        }
    }

    public class LoadingScreenView : BaseView
    {
        [SerializeField] private Slider sldLoading;
        public                   Slider SldLoading => this.sldLoading;
    }
}
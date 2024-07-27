namespace DIContexts
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using UIFeatures.MainScene;
    using UserData.Controller;

    public class MainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<MainScreenPresenter>();
            this.Container.Bind<InventoryManager>().AsSingle();
            this.Container.Bind<CharacterManager>().AsSingle();
        }
    }
}
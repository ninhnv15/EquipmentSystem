namespace DIContexts
{
    using DataManager.MasterData;
    using GameFoundation.Scripts;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundationBridge;
    using UnityEngine.EventSystems;
    using Zenject;

    public class GameProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Common systems and services
            GameFoundationInstaller.Install(this.Container);

            //Rebind SceneDirector
            this.Container.Bind<GameSceneDirector>().AsSingle().NonLazy();
            this.Container.Rebind<SceneDirector>().FromResolveGetter<GameSceneDirector>(data => data).AsCached();

            //Local User data
            this.Container.Bind<MasterDataManager>().AsSingle();

            //Common Event System
            this.Container.Bind<EventSystem>().FromComponentInNewPrefabResource(nameof(EventSystem)).AsSingle().NonLazy();
        }
    }
}
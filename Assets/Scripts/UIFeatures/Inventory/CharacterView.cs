﻿namespace UIFeatures.Inventory
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Model;
    using Zenject;

    public class CharacterView : TViewMono
    {
        [field: SerializeField] public Image              ImgCharacterPortrait { get; private set; }
        [field: SerializeField] public TextMeshProUGUI    TxtCharacterName     { get; private set; }
        [field: SerializeField] public List<SlotItemView> SlotItemViews        { get; private set; }

        [field: SerializeField] public Transform StatHolder { get; private set; }
    }

    public class CharacterPresenter : BaseUIItemPresenter<CharacterView, Character>
    {
        private readonly DiContainer             diContainer;
        private          List<SlotItemPresenter> slotItemPresenters = new();
        private          List<StatItemPresenter> statItemPresenters = new();

        public CharacterPresenter(IGameAssets gameAssets, DiContainer diContainer) : base(gameAssets) { this.diContainer = diContainer; }

        public override void BindData(Character data)
        {
            // Setup character info
            this.View.TxtCharacterName.text = data.StaticData.Name;

            this.View.ImgCharacterPortrait.gameObject.SetActive(false);
            this.GetCharacterPortrait(data.StaticData.Avatar).ContinueWith(sprite =>
            {
                this.View.ImgCharacterPortrait.sprite = sprite;
                this.View.ImgCharacterPortrait.gameObject.SetActive(true);
            });


            // Setup slot item views
            foreach (var slotItemView in this.View.SlotItemViews)
            {
                var slotItemPresenter = this.diContainer.Instantiate<SlotItemPresenter>();
                slotItemPresenter.SetView(slotItemView);
                this.slotItemPresenters.Add(slotItemPresenter);

                slotItemPresenter.BindData(data.SlotItems.GetValueOrDefault(slotItemView.ItemType));
            }

            // Setup stats
            if (this.statItemPresenters == null || this.statItemPresenters.Count == 0)
            {
                foreach (var statDataElement in data.StatsDictionary.Values)
                {
                    if (!statDataElement.StatRecord.IsShow) continue;
                    SetupStatItemView(statDataElement.StatRecord.StatItemViewPath, statDataElement).Forget();
                }
            }
        }

        private async UniTask<Sprite> GetCharacterPortrait(string portraitPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(portraitPath); }

        private async UniTask SetupStatItemView(string prefabPath, StatDataElement model)
        {
            var statItemPresenter = this.diContainer.Instantiate<StatItemPresenter>();

            statItemPresenter.SetView(Object.Instantiate(await this.GameAssets.LoadAssetAsync<GameObject>(prefabPath), this.View.StatHolder).GetComponent<BaseStatItemView>());
            statItemPresenter.BindData(model);
            this.statItemPresenters.Add(statItemPresenter);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var slotItemPresenter in this.slotItemPresenters)
            {
                slotItemPresenter.Dispose();
            }

            foreach (var statItemPresenter in this.statItemPresenters)
            {
                statItemPresenter.Dispose();
            }
        }
    }
}
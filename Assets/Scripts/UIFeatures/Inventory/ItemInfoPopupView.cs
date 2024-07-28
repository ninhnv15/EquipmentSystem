namespace UIFeatures.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Controller;
    using UserData.Model;
    using Zenject;

    public class ItemInfoPopupView : BaseView
    {
        [field: SerializeField] public TextMeshProUGUI TxtTitle       { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtDescription { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtItemName    { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtStatInfo    { get; private set; }

        [field: SerializeField] public Image ImgItemIcon { get; private set; }

        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public Button BtnEquip { get; private set; }
        [field: SerializeField] public Button BtnSell  { get; private set; }
    }

    [PopupInfo(nameof(ItemInfoPopupView))]
    public class ItemInfoPopupPresenter : BasePopupPresenter<ItemInfoPopupView, Item>
    {
        private readonly StatBlueprint    statBlueprint;
        private readonly IGameAssets      gameAssets;
        private readonly CharacterManager characterManager;
        private          Item             currentItem;

        public ItemInfoPopupPresenter(SignalBus signalBus, ILogService logService, StatBlueprint statBlueprint, IGameAssets gameAssets, CharacterManager characterManager) : base(signalBus, logService)
        {
            this.statBlueprint    = statBlueprint;
            this.gameAssets       = gameAssets;
            this.characterManager = characterManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();

            this.View.BtnClose.onClick.AddListener(this.CloseView);
            this.View.BtnEquip.onClick.AddListener(this.OnClickEquipButton);
            this.View.BtnSell.onClick.AddListener(this.OnClickSellButton);
        }

        public override UniTask BindData(Item itemData)
        {
            this.currentItem                = itemData;
            this.View.TxtTitle.text         = itemData.StaticData.ItemType.ToString();
            this.View.TxtDescription.text   = itemData.StaticData.Description;
            this.View.TxtItemName.text      = itemData.StaticData.Name;
            this.View.BtnEquip.interactable = true;

            // format stat dictionary to string like
            // "StatName           +StatValue"
            this.View.TxtStatInfo.text = string.Join("\n", itemData.StaticData.StatToValue.Select(stat =>
                $"{this.statBlueprint.GetDataById(stat.Key).Name,10} {"+" + stat.Value,20}"));

            this.GetItemImage(itemData.StaticData.Icon).ContinueWith(sprite => this.View.ImgItemIcon.sprite = sprite);

            return UniTask.CompletedTask;
        }

        private async UniTask<Sprite> GetItemImage(string iconPath) { return await this.gameAssets.LoadAssetAsync<Sprite>(iconPath); }

        private void OnClickSellButton() { throw new System.NotImplementedException(); }
        private void OnClickEquipButton()
        {
            if (this.characterManager.TryEquipItem(this.currentItem))
            {
                this.View.BtnEquip.interactable = false;
            }
        }
    }
}
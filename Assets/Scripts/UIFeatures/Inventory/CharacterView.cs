﻿namespace UIFeatures.Inventory
{
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Controller;
    using UserData.Model;
    using Zenject;

    public class CharacterView : TViewMono
    {
        [field: SerializeField] public Image                  ImgCharacterPortrait   { get; private set; }
        [field: SerializeField] public TextMeshProUGUI        TxtCharacterName       { get; private set; }
        [field: SerializeField] public List<SlotItemView>     SlotItemViews          { get; private set; }
        [field: SerializeField] public ConsumableSlotItemView ConsumableSlotItemView { get; private set; }

        [field: SerializeField] public Transform StatHolder { get; private set; }
        [field: SerializeField] public Transform VfxHolder  { get; private set; }
    }

    public class CharacterPresenter : BaseUIItemPresenter<CharacterView, Character>
    {
        private readonly DiContainer                             diContainer;
        private readonly CharacterManager                        characterManager;
        private readonly InventoryManager                        inventoryManager;
        private readonly InventoryScreenPresenter                inventoryScreenPresenter;
        private          List<SlotItemPresenter>                 slotItemPresenters = new();
        private          Dictionary<StatType, StatItemPresenter> statItemPresenters = new();

        private Character characterData;
        public CharacterPresenter(IGameAssets gameAssets, DiContainer diContainer, CharacterManager characterManager, InventoryManager inventoryManager,
            InventoryScreenPresenter inventoryScreenPresenter) : base(gameAssets)
        {
            this.diContainer              = diContainer;
            this.characterManager         = characterManager;
            this.inventoryManager         = inventoryManager;
            this.inventoryScreenPresenter = inventoryScreenPresenter;
        }

        public override void BindData(Character data)
        {
            this.characterData = data;
            // Setup character info
            this.View.TxtCharacterName.text = data.StaticData.Name;

            this.View.ImgCharacterPortrait.gameObject.SetActive(false);
            this.GetCharacterPortrait(data.StaticData.Avatar).ContinueWith(sprite =>
            {
                this.View.ImgCharacterPortrait.sprite = sprite;
                this.View.ImgCharacterPortrait.gameObject.SetActive(true);
            });

            this.SubscribeStatValue(StatType.Health);
            this.SubscribeStatValue(StatType.Mana);

            // Setup slot item views
            foreach (var slotItemView in this.View.SlotItemViews)
            {
                var slotItemPresenter = this.diContainer.Instantiate<SlotItemPresenter>();
                slotItemPresenter.SetView(slotItemView);
                this.slotItemPresenters.Add(slotItemPresenter);

                slotItemPresenter.BindData(data.GetOrAddSlotItem(slotItemView.ItemType));
                slotItemView.OnDropItem = () => { this.OnDropItem(slotItemView.ItemType); };
            }

            this.View.ConsumableSlotItemView.OnDropItem = () => { this.OnDropItem(ItemType.Consumable); };

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
        private void SubscribeStatValue(StatType statType)
        {
            if (this.characterData.StatsDictionary.TryGetValue(statType, out var statDataElement))
            {
                var currentValue = statDataElement.CurrentValue.Value;
                statDataElement.CurrentValue.Subscribe( newValue =>
                {
                    if (newValue > currentValue)
                    {
                        ObjectPoolManager.Instance.Spawn($"BuffVfx_{statType}").ContinueWith(vfx =>
                        {
                            vfx.transform.SetParent(this.View.VfxHolder,false);
                            vfx.transform.localScale = Vector3.one;
                        });
                    }

                    currentValue = newValue;
                });
            }
        }

        private async UniTask<Sprite> GetCharacterPortrait(string portraitPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(portraitPath); }

        private async UniTask SetupStatItemView(string prefabPath, StatDataElement model)
        {
            var statItemPresenter = this.diContainer.Instantiate<StatItemPresenter>();

            statItemPresenter.SetView(Object.Instantiate(await this.GameAssets.LoadAssetAsync<GameObject>(prefabPath), this.View.StatHolder).GetComponent<BaseStatItemView>());
            statItemPresenter.BindData(model);
            this.statItemPresenters.Add(model.Type, statItemPresenter);
        }

        private void OnDropItem(ItemType slotItemType)
        {
            var selectedItem = this.inventoryScreenPresenter.GetSelectedItem();
            if (selectedItem.StaticData.ItemType == slotItemType)
            {
                if (this.characterManager.TryEquipItem(selectedItem, out var oldItem))
                {
                    this.inventoryManager.RemoveItem(selectedItem);
                    this.inventoryManager.AddItem(oldItem);

                    this.inventoryScreenPresenter.UnSelectCurrentItem();
                    this.inventoryScreenPresenter.RefreshListItemView();
                }
            }
        }

        public void TryItem(Item newItem)
        {
            var statChanges = newItem.StaticData.StatToValue.ToDictionary(x => x.Key, x => x.Value);

            if (this.characterData.SlotItems.TryGetValue(newItem.StaticData.ItemType, out var slotItem) && slotItem.Item.Value != null)
            {
                foreach (var statType in newItem.StaticData.StatToValue.Keys)
                {
                    if (slotItem.Item.Value.StaticData.StatToValue.TryGetValue(statType, out var statValue))
                    {
                        statChanges[statType] -= statValue;
                    }
                }
            }

            foreach (var statItemPresenter in this.statItemPresenters)
            {
                var statDataElement = this.characterData.StatsDictionary[statItemPresenter.Key];
                if (statChanges.TryGetValue(statDataElement.Type, out var changeValue))
                {
                    statItemPresenter.Value.SetChangeValue(changeValue);
                }

                if (statChanges.TryGetValue(statDataElement.StatRecord.ClampedBy, out var changeMaxValue))
                {
                    statItemPresenter.Value.SetChangeMaxValue(changeMaxValue);
                }
            }
        }

        public void ResetChangeValue()
        {
            foreach (var statItemPresenter in this.statItemPresenters.Values)
            {
                statItemPresenter.ResetChangeValue();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var slotItemPresenter in this.slotItemPresenters)
            {
                slotItemPresenter.Dispose();
            }

            foreach (var statItemPresenter in this.statItemPresenters.Values)
            {
                statItemPresenter.Dispose();
            }
        }
    }
}
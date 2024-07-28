namespace UIFeatures.Inventory
{
    using System;
    using Blueprints;
    using Coffee.UIEffects;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UserData.Model;

    public class SlotItemView : TViewMono, IDropHandler
    {
        [field: SerializeField] public ItemType ItemType     { get; private set; }
        [field: SerializeField] public Image    IconItem     { get; private set; }
        [field: SerializeField] public Image    IconItemType { get; private set; }

        public Action OnDropItem;
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log($"OnDrop {this.ItemType} - {eventData.pointerDrag.name}");
            this.OnDropItem?.Invoke();
        }
    }

    public class SlotItemPresenter : BaseUIItemPresenter<SlotItemView, SlotItem>
    {
        private IDisposable itemObservable;
        public SlotItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(SlotItem data)
        {
            var isFirstTime = true;
            this.itemObservable = data.Item.Subscribe(item =>
            {
                if (item != null)
                {
                    this.View.IconItem.gameObject.SetActive(false);
                    this.View.IconItemType.gameObject.SetActive(false);
                    this.GetItemIcon(data.Item.Value.StaticData.Icon).ContinueWith(sprite =>
                    {
                        this.View.IconItem.sprite = sprite;
                        this.View.IconItem.gameObject.SetActive(true);

                        if (isFirstTime)
                        {
                            isFirstTime = false;
                            return;
                        }

                        this.View.IconItem.transform.DOKill();
                        this.View.IconItem.transform.localScale = Vector3.one * 1.5f;
                        this.View.IconItem.transform.DOScale(1.0f, 0.25f).SetEase(Ease.OutElastic).OnComplete(() => { this.View.IconItem.GetComponent<UIShiny>().Play(); });
                    });
                }
                else
                {
                    this.View.IconItem.gameObject.SetActive(false);
                    this.View.IconItemType.gameObject.SetActive(true);
                }
            });
        }

        private async UniTask<Sprite> GetItemIcon(string iconItemPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(iconItemPath); }

        public override void Dispose()
        {
            base.Dispose();
            this.itemObservable?.Dispose();
            this.View.OnDropItem = null;
        }
    }


    public class SlotItemModel
    {
        public Item Item { get; set; }
    }
}
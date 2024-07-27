namespace UIFeatures.Inventory
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UserData.Model;

    public class InventoryItemView : TViewMono, IPointerClickHandler
    {
        [field: SerializeField] public Image           ImgItemIcon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAmount   { get; private set; }
        [field: SerializeField] public GameObject      FocusObject { get; private set; }

        public Action OnClick;

        public void OnPointerClick(PointerEventData eventData) { this.OnClick?.Invoke(); }
    }

    public class InventoryItemPresenter : BaseUIItemPresenter<InventoryItemView, InventoryItemModel>
    {
        private IDisposable selectObservable;
        public InventoryItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(InventoryItemModel data)
        {
            this.View.ImgItemIcon.gameObject.SetActive(false);
            this.GetItemIcon(data.Item.StaticData.Icon).ContinueWith(sprite =>
            {
                this.View.ImgItemIcon.sprite = sprite;
                this.View.ImgItemIcon.gameObject.SetActive(true);
            });

            if (data.Item.Amount > 1)
            {
                this.View.TxtAmount.gameObject.SetActive(true);
                this.View.TxtAmount.text = data.Item.Amount.ToString();
            }
            else
            {
                this.View.TxtAmount.gameObject.SetActive(false);
            }

            this.View.OnClick     = () => { data.OnSelected?.Invoke(); };
            this.selectObservable = data.IsSelected.Subscribe(this.SetSelectItemState);
        }

        private async UniTask<Sprite> GetItemIcon(string iconItemPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(iconItemPath); }


        private void SetSelectItemState(bool isSelected) { this.View.FocusObject.SetActive(isSelected); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.OnClick = null;
            selectObservable?.Dispose();
        }
    }

    public class InventoryItemModel
    {
        public BoolReactiveProperty IsSelected { get; }
        public Action               OnSelected { get; set; }
        public Item                 Item       { get; }

        public InventoryItemModel(Item item, bool isSelected = false)
        {
            this.Item       = item;
            this.IsSelected = new BoolReactiveProperty(isSelected);
        }
    }
}
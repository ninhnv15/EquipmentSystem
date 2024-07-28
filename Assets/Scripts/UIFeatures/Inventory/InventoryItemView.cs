namespace UIFeatures.Inventory
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UserData.Model;

    [Serializable]
    public class DragDropAnimationSettings
    {
        public float dragThreshold = 0.5f;

        public float beginDragScale    = 1.5f;
        public float beginDragDuration = 0.1f;
        public float endDragScale      = 1.0f;
        public float endDragDuration   = 0.25f;
        public Ease  endDragEase       = Ease.OutBounce;
    }

    public class InventoryItemView : TViewMono, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField] public Image           ImgItemIcon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAmount   { get; private set; }
        [field: SerializeField] public GameObject      FocusObject { get; private set; }

        [SerializeField] private DragDropAnimationSettings dragDropAnimationSettings;

        public Action     OnClick;
        public Transform  InventoryRoot       { get; set; }
        public InventoryItemGridViewAdapter InventoryScrollRect { get; set; }

        private RectTransform itemIconRectTransform;
        private Tweener       scaleTween;
        private IDisposable   clickObservable;
        private bool          isDraggingItem;

        private void Start() { this.itemIconRectTransform = this.ImgItemIcon.GetComponent<RectTransform>(); }

        public void OnInitializePotentialDrag(PointerEventData eventData) { ExecuteEvents.Execute(InventoryScrollRect.gameObject, eventData, ExecuteEvents.initializePotentialDrag); }
        public void OnBeginDrag(PointerEventData eventData)
        {
            ExecuteEvents.Execute(InventoryScrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
            Debug.Log("Begin Drag");
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (this.isDraggingItem)
            {
                this.itemIconRectTransform.anchoredPosition += eventData.delta;
            }
            else
            {
                ExecuteEvents.Execute(InventoryScrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ExecuteEvents.Execute(InventoryScrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
            this.ResetDraggedItem();
            Debug.Log("OnEndDrag");
        }
        private void ResetDraggedItem()
        {
            if (this.isDraggingItem)
            {
                this.itemIconRectTransform.SetParent(this.transform);
                this.ImgItemIcon.raycastTarget = true;
                this.scaleTween.Kill();
                this.itemIconRectTransform.DOScale(this.dragDropAnimationSettings.endDragScale, this.dragDropAnimationSettings.endDragDuration).SetEase(this.dragDropAnimationSettings.endDragEase);
                this.itemIconRectTransform.anchoredPosition = Vector2.zero;

                this.isDraggingItem = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.OnClick?.Invoke();

            // using UniRx to observer drag threshold
            this.clickObservable = Observable.Timer(TimeSpan.FromSeconds(dragDropAnimationSettings.dragThreshold)).Subscribe(_ =>
            {
                this.isDraggingItem = true;

                this.itemIconRectTransform.SetParent(this.InventoryRoot);
                this.ImgItemIcon.raycastTarget = false;
                this.scaleTween                = this.itemIconRectTransform.DOScale(dragDropAnimationSettings.beginDragScale, dragDropAnimationSettings.beginDragDuration);

            }).AddTo(this);
        }
        public void OnPointerExit(PointerEventData eventData)             { this.clickObservable?.Dispose(); }
        public void OnPointerUp(PointerEventData eventData)
        {
            this.clickObservable?.Dispose();
            ResetDraggedItem();
        }
    }

    public class InventoryItemPresenter : BaseUIItemPresenter<InventoryItemView, InventoryItemModel>
    {
        private IDisposable selectObservable;
        public InventoryItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(InventoryItemModel data)
        {
            this.View.InventoryRoot       = data.InventoryRoot;
            this.View.InventoryScrollRect = data.InventoryScrollRect;
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
        public BoolReactiveProperty IsSelected          { get; }
        public Action               OnSelected          { get; set; }
        public Item                 Item                { get; }
        public Transform            InventoryRoot       { get; set; }
        public InventoryItemGridViewAdapter           InventoryScrollRect { get; set; }

        public InventoryItemModel(Item item, bool isSelected = false)
        {
            this.Item       = item;
            this.IsSelected = new BoolReactiveProperty(isSelected);
        }
    }
}
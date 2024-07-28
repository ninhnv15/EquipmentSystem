namespace UIFeatures.Inventory
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ConsumableSlotItemView : MonoBehaviour, IDropHandler
    {
        public Action OnDropItem;
        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log($"OnDrop ConsumableSlotItemView - {eventData.pointerDrag.name}");
            this.OnDropItem?.Invoke();
        }
    }
}
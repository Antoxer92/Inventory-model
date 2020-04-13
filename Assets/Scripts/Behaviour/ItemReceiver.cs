using System;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemReceiver : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{  
    protected virtual Type ItemHolderType => throw new System.NotImplementedException();
    public virtual SlotType SlotType => throw new System.NotImplementedException();
    protected virtual void OnMatchedItemEnter(IItemHolder itemHolder) {}
    protected virtual void OnDismatchedItemEnter(IItemHolder itemHolder) {}
    protected virtual void OnItemExit(IItemHolder itemHolder) {}
    protected virtual void Receive(IItemHolder itemHolder) {}
    protected virtual void Reject(IItemHolder itemHolder) {}
    protected virtual bool CanReceive(IItemHolder itemHolder) { return true; }
    
    public void OnDrop(PointerEventData eventData)
    {
        DragableInventoryItem item = eventData.pointerDrag?.GetComponent<DragableInventoryItem>();
        if (item == null)
            return;

        if (IsMatch(item.ItemHolder))
        {
            Receive(item.ItemHolder);
            item.OnEndDrag(this);
        }
        else
        {
            Reject(item.ItemHolder);
            item.OnEndDrag();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DragableInventoryItem item = eventData.pointerDrag?.GetComponent<DragableInventoryItem>();
        if (item != null && (object)item.ItemHolder != this)

        if (IsMatch(item.ItemHolder))
            OnMatchedItemEnter(item.ItemHolder);
        else
            OnDismatchedItemEnter(item.ItemHolder);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DragableInventoryItem item = eventData.pointerDrag?.GetComponent<DragableInventoryItem>();
        if (item != null && (object)item.ItemHolder != this)
            OnItemExit(item.ItemHolder);
    }

    private bool IsMatch(IItemHolder itemHolder)
    {
        return itemHolder.GetType() == ItemHolderType && SlotType.HasFlag(itemHolder.Asset.TargetSlot) && CanReceive(itemHolder);
    }
}
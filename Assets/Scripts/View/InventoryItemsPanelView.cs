using System;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemsPanelView : ItemReceiver
{
    [SerializeField]
    private Image border_img;
    
    [Header("Colors")]
    [SerializeField]
    private Color idle_color = Color.grey;
    [SerializeField]
    private Color match_color = Color.yellow;
    
    override protected Type ItemHolderType => typeof(PersonageSlotView);
    override public SlotType SlotType => SlotType.Any;
    
    override protected void Receive(IItemHolder itemHolder)
    {
        if (itemHolder.Asset is DressableAsset)
            DressController.TakeOff(itemHolder.Asset.TargetSlot, itemHolder.Personage);

        border_img.color = idle_color;
    }

    override protected void OnMatchedItemEnter(IItemHolder itemHolder)
    {
        border_img.color = match_color;
    }

    override protected void OnDismatchedItemEnter(IItemHolder itemHolder)
    {
        border_img.color = idle_color;
    }

    override protected void OnItemExit(IItemHolder itemHolder)
    {
        border_img.color = idle_color;
    }

    void Start()
    {
        border_img.color = idle_color;
    }
}
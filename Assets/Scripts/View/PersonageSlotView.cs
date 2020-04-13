using System;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class PersonageSlotView : ItemReceiver, IItemHolder
{
    [SerializeField]
    private SlotType slot_type;
    [SerializeField]
    private Image border_img;
    [SerializeField]
    private Image icon_img;
    [SerializeField]
    private Image bgr_img;
    [SerializeField]
    private DragableInventoryItem dragable_item;

    [Header("Colors")]
    [SerializeField]
    private Color idle_color = Color.grey;
    [SerializeField]
    private Color capture_color = Color.yellow;
    [SerializeField]
    private Color match_color = Color.green;
    [SerializeField]
    private Color dismatch_color = Color.red;

    private Transform _transform;
    private SlotItem _slot_item;
    private PersonageModel _personage_model;
    private bool _is_capture;

    override protected Type ItemHolderType => typeof(InventoryItemView);
    override public SlotType SlotType => slot_type;

    public DressableAsset Asset => _slot_item?.Asset as DressableAsset;
    public PersonageModel Personage => _personage_model;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    void Start()
    {
        border_img.color = idle_color;
    }
    
    public void Init(PersonageModel personageModel, Transform dragTransform, SlotItem slotItem = null)
    {
        _slot_item = slotItem;
        _personage_model = personageModel;
        _personage_model.OnPutOnItem += OnPutOnSlotItem;
        _personage_model.OnTakeOffItem += OnTakeOffSlotItem;
        dragable_item.Init(this, _transform, dragTransform);

        UpdateView();
    }

    private void UpdateView()
    {
        dragable_item.UpdateView();
        
        if (_slot_item == null)
        {
            icon_img.sprite = null;
            icon_img.enabled = false;
            bgr_img.enabled = true;
        }
        else
        {
            icon_img.sprite = _slot_item.Asset.Icon;
            icon_img.enabled = true;
            bgr_img.enabled = false;
        }
    }

    internal void Clear()
    {
        _personage_model.OnPutOnItem -= OnPutOnSlotItem;
        _personage_model.OnTakeOffItem -= OnTakeOffSlotItem;
        _slot_item = null;
        _personage_model = null;
        dragable_item.Clear();
    }

    private void OnTakeOffSlotItem(SlotItem item)
    {
        if (_slot_item != item)
            return;

        _slot_item = null;
        UpdateView();
    }

    private void OnPutOnSlotItem(SlotItem item)
    {
        if (slot_type != item.SlotType)
            return;

        _slot_item = item;
        UpdateView();
    }

    public void OnBeginDrag()
    {
        _is_capture = true;
        border_img.color = capture_color;
    }

    public void OnEndDrag(ItemReceiver itemReceiver)
    {
        _is_capture = false;
        border_img.color = idle_color;
    }

    override protected void OnMatchedItemEnter(IItemHolder itemHolder)
    {
        border_img.color = match_color; 
    }

    override protected void OnDismatchedItemEnter(IItemHolder itemHolder)
    {
        border_img.color = dismatch_color; 
    }

    override protected void OnItemExit(IItemHolder itemHolder)
    {
        if (!_is_capture)
            border_img.color = idle_color;
    }

    override protected void Receive(IItemHolder itemHolder)
    {
        if (itemHolder != null)
            DressController.PutOn(itemHolder.Asset, itemHolder.Asset.TargetSlot, itemHolder.Personage);

        border_img.color = idle_color;
    }

    override protected void Reject(IItemHolder itemHolder)
    {
        border_img.color = idle_color;
    }

    override protected bool CanReceive(IItemHolder itemHolder)
    {
        return DressController.CanDress(itemHolder.Asset, SlotType);
    }
}

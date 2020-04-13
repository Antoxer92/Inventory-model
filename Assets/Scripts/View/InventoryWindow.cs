using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : BaseWindow
{
    [Header("MainPanel")]
    [SerializeField]
    private Button close_btn;

    [Header("PersonageSlotsPanel")]
    [SerializeField]
    private List<PersonageSlotView> slot_views;
    
    [Header("InventroryItemsPanel")]
    [SerializeField]
    private ScrollRect scroll_panel;
    [SerializeField]
    private GameObject item_prefab;
    [SerializeField]
    private int rows = -1;
    [SerializeField]
    private int columns = 4;
    [SerializeField]
    private RectOffset padding;

    private Transform _transform;
    private PersonageModel _personage_model;
    private UserInventory _user_inventory;
    private Queue<InventoryItemModel> _creation_items_queue;
    private List<InventoryItemView> _inventory_item_views;
    private Stack<InventoryItemView> _inventory_item_views_pool;
    private Transform _items_container_transform;
    private DragableInventoryItem _drag_item;
    private int _total_items_count;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _inventory_item_views = new List<InventoryItemView>();
        _inventory_item_views_pool = new Stack<InventoryItemView>();
        _items_container_transform = scroll_panel.content.GetComponent<Transform>();
    }
    
    public void Init(PersonageModel personageModel, UserInventory userInventory)
    {
        _personage_model = personageModel;
        _user_inventory = userInventory;
    }

    override protected void SelfOpen()
    {
        InitMainPanel();
        InitPersonagePanel();
        InitInventoryPanel();
    }

    private void InitMainPanel()
    {
        close_btn.onClick.AddListener(() => G.self.window_manager.CloseWindow(this));
    }

    private void InitPersonagePanel()
    {
        for (int i = 0; i < slot_views.Count; i++)
        {
            _personage_model.GetSlotItem(slot_views[i].SlotType, out SlotItem slotItem);
            slot_views[i].Init(_personage_model, _transform, slotItem);
        }
    }

    private void InitInventoryPanel()
    {
        var items = _user_inventory.GetItems();
        _total_items_count = items.Count;
        _creation_items_queue = new Queue<InventoryItemModel>(items);
        SetContentSize();
        _user_inventory.OnIncrease += OnItemIncrease;
        _user_inventory.OnDecrease += OnItemDecrease;
        scroll_panel.onValueChanged.AddListener(OnScroll);
    }

    private void SetContentSize()
    {
        var prefab_rect = item_prefab.GetComponent<RectTransform>().rect;
        var content_rect = scroll_panel.content;
        var total_columns = columns != -1 ? columns : Mathf.CeilToInt(_total_items_count / (float)rows);
        var total_rows = rows != -1 ? rows : Mathf.CeilToInt(_total_items_count / (float)columns);
        var width = padding.left + total_columns * prefab_rect.width + padding.right;
        var height = padding.top + total_rows * prefab_rect.height + padding.bottom;

        scroll_panel.content.sizeDelta = new Vector2(width, height);
    }

    void Update()
    {
        CreateInventoryItems();
    }

    private void CreateInventoryItems()
    {
        if (_creation_items_queue?.Count > 0)
            CreateInventoryItem();
    }

    private void CreateInventoryItem()
    {
        var item_index = _inventory_item_views.Count;
        var inventory_item = _creation_items_queue.Dequeue();
        var inventory_item_view = _inventory_item_views_pool.Count > 0 ? _inventory_item_views_pool.Pop() : Instantiate(item_prefab, _items_container_transform).GetComponent<InventoryItemView>();
        inventory_item_view.Init(inventory_item, _personage_model, _transform);
        _inventory_item_views.Add(inventory_item_view);
        MoveItem(item_index, inventory_item_view);
    }

    private void MoveItems(int index, bool with_tween = false)
    {
        for (int i = index; i < _inventory_item_views.Count; i++)
            MoveItem(i, _inventory_item_views[i], with_tween);
    }

    private void MoveItem(int index, InventoryItemView item, bool with_tween = false)
    {
        int row_index = columns != -1 ? Mathf.FloorToInt(index / columns) : index % rows;
        int column_index = columns != -1 ? index % columns : Mathf.FloorToInt(index / rows);
        var rect = item.GetRect();
        int previous_index = index - 1;
        Vector2 pos;

        InventoryItemView previous_slot = (previous_index >= 0) ? _inventory_item_views[previous_index] : null;
        if (previous_slot != null)
        {
            var previous_rect = previous_slot.GetRect();
            pos = new Vector2(column_index > 0 ? previous_rect.xMax : padding.left, -padding.top - row_index * rect.height);
        }
        else
            pos = new Vector2(padding.left, -padding.top);

        item.SetPos(pos, with_tween);
    }

    override protected void SelfClose()
    {
        DisposeMainPanel();
        DisposePersonagePanel();
        DisposeInventoryPanel();
    }

    private void DisposeMainPanel()
    {
        close_btn.onClick.RemoveAllListeners();
    }

    private void DisposeInventoryPanel()
    {
        for (int i = 0; i < _inventory_item_views.Count; i++)
        {
            _inventory_item_views[i].Clear();
            _inventory_item_views_pool.Push(_inventory_item_views[i]);
        }  

        _creation_items_queue.Clear();
        _inventory_item_views.RemoveRange(0, _inventory_item_views.Count);
        _user_inventory.OnIncrease -= OnItemIncrease;
        _user_inventory.OnDecrease -= OnItemDecrease;
    }

    private void DisposePersonagePanel()
    {
        scroll_panel.onValueChanged.RemoveListener(OnScroll);
        
        for (int i = 0; i < slot_views.Count; i++)
            slot_views[i].Clear();
    }

    private bool TryFindItemView(InventoryItemModel item, out int index, out InventoryItemView inventory_item_view)
    {
        for (int i = 0; i < _inventory_item_views.Count; i++)
        {
            if (!_inventory_item_views[i].IsMatch(item))
                continue;

            index = i;
            inventory_item_view = _inventory_item_views[i];
            return true;
        }

        index = -1;
        inventory_item_view = null;
        return false;
    }

    private void OnItemDecrease(InventoryItemModel item, int amount, int delta, bool removed)
    {
        if (!removed)
            return;

        if (!TryFindItemView(item, out int index, out InventoryItemView inventory_item_view))
            return;

        inventory_item_view.Clear();
        _inventory_item_views.Remove(inventory_item_view);
        _inventory_item_views_pool.Push(inventory_item_view);
        _total_items_count--;
        SetContentSize();
        MoveItems(index, true);
    }

    private void OnItemIncrease(InventoryItemModel item, int amount, int delta, bool added)
    {
        if (!added)
            return;

        _creation_items_queue.Enqueue(item);
        _total_items_count++;
        SetContentSize();
    }

    private void OnScroll(Vector2 pos)
    {
        /*  TODO: When there are too many inventory items we'll have to create their view instances by chunks on scroll event for best perfomance.
            Now we instantiate all of them every frame after opening window*/
    }
}
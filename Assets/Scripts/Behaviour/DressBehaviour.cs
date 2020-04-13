using System.Collections.Generic;
using Game;
using UnityEngine;

public class DressBehaviour : MonoBehaviour
{
    [SerializeField]
    private Slot[] slots;

    private PersonageModel personage_model;
    private Transform _transform;
    private Dictionary<SlotType, DressableObject> _dressable_objects;
    private Dictionary<SlotElementPlacement, Slot> _slots_map;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _dressable_objects = new Dictionary<SlotType, DressableObject>();
        _slots_map = new Dictionary<SlotElementPlacement, Slot>();

        FillSlotsMap();
    }

    private void FillSlotsMap()
    {
        for (int i = 0; i < slots.Length; i++)
            _slots_map.Add(slots[i].Placement, slots[i]);
    }

    public void Init(PersonageModel personageModel)
    {
        personage_model = personageModel;
        personage_model.OnPutOnItem += OnPutOnItem;
        personage_model.OnTakeOffItem += OnTakeOffItem;
        InitializeItems();
    }

    public void Dispose()
    {
        if (personage_model == null)
            return;
        
        personage_model.OnPutOnItem -= OnPutOnItem;
        personage_model.OnTakeOffItem -= OnTakeOffItem;
        personage_model = null;
    }

    private void InitializeItems()
    {
        var items = personage_model.GetItems();
        
        for (int i = 0; i < items.Count; i++)
            PutOnItem(items[i]);
    }

    private void OnTakeOffItem(SlotItem slotItem)
    {
        TakeOffItem(slotItem);
    }

    private void OnPutOnItem(SlotItem slotItem)
    {
        PutOnItem(slotItem);
    }

    private void PutOnItem(SlotItem slotItem)
    {
        if (slotItem.Asset.Prefab == null)
            UnityEngine.Debug.LogError(string.Format("Property \"Prefab\" doesn't set in stuff model {0}, so this item can't be displayed on personage", slotItem.Asset.Classname));
        
        GameObject clothesObject = Instantiate(slotItem.Asset.Prefab, _transform);
        DressableObject dressableObject = clothesObject.GetComponent<DressableObject>();

        if (dressableObject == null)
            UnityEngine.Debug.LogError(string.Format("Prefab for stuff model {0} don't has script \"DressableObject\", so this item can't be displayed on personage", slotItem.Asset.Classname));  

        _dressable_objects.Add(slotItem.SlotType, dressableObject);
        
        for (int i = 0; i < dressableObject.Elements.Length; i++)
            PutOnElement(dressableObject.Elements[i]);
    }

    private void TakeOffItem(SlotItem slotItem)
    {
        if (!_dressable_objects.TryGetValue(slotItem.SlotType, out DressableObject dressableObject))
            return;
        
        for (int i = 0; i < dressableObject.Elements.Length; i++)
            TakeOffElement(dressableObject.Elements[i]);

        dressableObject.transform.SetParent(null);
        Destroy(dressableObject.gameObject);

        _dressable_objects.Remove(slotItem.SlotType);
    }

    private void PutOnElement(SlotElement slotElement)
    {
        if (slotElement.GameObject == null)
            return;

        if (!_slots_map.TryGetValue(slotElement.Placement, out Slot slot))
        {
            UnityEngine.Debug.LogError(string.Format("Stuff model has clothes for slot {0}, but StuffPrefab doesn't have such slot", slotElement.Placement.ToString()));
            return;
        }
            
        Transform elementTransform = slotElement.GameObject.transform;
        Rigidbody body = slotElement.GameObject.GetComponent<Rigidbody>();
        if (body != null)
            body.isKinematic = true;

        elementTransform.SetParent(slot.Container.transform);
        elementTransform.localPosition = slotElement.Position;
        elementTransform.localEulerAngles = slotElement.Rotation;
    }

    private void TakeOffElement(SlotElement slotElement)
    {
        slotElement.GameObject.transform.SetParent(null);
        Destroy(slotElement.GameObject);
    }

    private bool TryGetSlot(SlotElement slotElement, out Slot slot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Placement != slotElement.Placement)
                continue;
            
            slot = slots[i];
            return true;
        }
        
        slot = null;
        
        return false;
    }

    void OnMouseUpAsButton()
    {
        G.self.window_manager.OpenWindow<InventoryWindow>((ui) =>
        { 
          ui.Init(personage_model, G.self.user_inventory);
        });
    }
}

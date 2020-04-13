using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Slot
    {
        [SerializeField]
        private GameObject container;
        [SerializeField]
        private SlotElementPlacement placement;

        public GameObject Container { get => container; }
        public SlotElementPlacement Placement { get => placement; }
    }

    public class SlotItem
    {
        public SlotType SlotType { get; private set; }
        public DressableAsset Asset { get; private set; }

        public SlotItem(SlotType slotType, DressableAsset asset)
        {
            SlotType = slotType;
            Asset = asset;
        }
    }

    public class PersonageModel : SpawnableModel
    {
        public event Action<SlotItem> OnPutOnItem;
        public event Action<SlotItem> OnTakeOffItem;
        public Personage Asset {get; private set; }

        private DressBehaviour dress_behaviour;
        private Dictionary<SlotType, SlotItem> items;

        public PersonageModel(Personage asset) : base(asset.GameObject)
        {
            Asset = asset;
            Init();
        }

        private void Init()
        {
            items = new Dictionary<SlotType, SlotItem>();
        }

        public bool GetSlotItem(SlotType slotType, out SlotItem slotItem)
        {
            return items.TryGetValue(slotType, out slotItem);
        }

        override protected void AfterInstantiateActions()
        {
            dress_behaviour = GameObject.GetComponent<DressBehaviour>();
            dress_behaviour.Init(this);
        }

        override protected void BeforeDisembodyActions()
        {
            dress_behaviour.Dispose();
            dress_behaviour = null;
        }

        public bool TryGetConflictSlotItems(DressableAsset slotAsset, out List<SlotItem> conflictItems)
        {
            conflictItems = new List<SlotItem>();
            
            foreach (KeyValuePair<SlotType, SlotItem> pair in items)
            {
                if (DressController.IsConflict(slotAsset, pair.Value.Asset))
                    conflictItems.Add(pair.Value);
            }

            return conflictItems.Count > 0;
        }

        public void PutOnItem(SlotType slotType, DressableAsset slotAsset)
        {
            var slotItem = new SlotItem(slotType, slotAsset);
            items.Add(slotType, slotItem);
            OnPutOnItem?.Invoke(slotItem);
        }

        public void TakeOffItem(SlotItem slotItem)
        {
            items.Remove(slotItem.SlotType);
            OnTakeOffItem?.Invoke(slotItem);
        }

        public IList<SlotItem> GetItems()
        {
            SlotItem[] slotItems = new SlotItem[items.Count];
            items.Values.CopyTo(slotItems, 0);
            return slotItems;
        }
    }
}
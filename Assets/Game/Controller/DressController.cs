using System;
using System.Collections.Generic;

namespace Game
{
    public static class DressController
    {
        public static bool CanDress(DressableAsset asset, SlotType slotType)
        {
            return asset.TargetSlot == slotType;
        }

        public static bool IsConflict(DressableAsset asset1, DressableAsset asset2)
        {
            for (int i = 0; i < asset1.OccupiedSlots.Length; i++)
            {
                for (int j = 0; j < asset2.OccupiedSlots.Length; j++)
                {
                    if (asset1.OccupiedSlots[i] == asset2.OccupiedSlots[j])
                        return true;
                }
            }
            
            return false; 
        }

        private static void TakeOff(SlotItem slotItem, PersonageModel personage)
        {
            G.self.user_inventory.Increase(slotItem.Asset.Classname);
            personage.TakeOffItem(slotItem);
        }

        public static void TakeOff(SlotType slotType, PersonageModel personage)
        {
            if (!personage.GetSlotItem(slotType, out SlotItem slotItem))
            {
                UnityEngine.Debug.LogError(string.Format("Can't take off item {0}. User personage don't dress this item"));
                return;
            }

           TakeOff(slotItem, personage);
        }

        public static void PutOn(DressableAsset asset, SlotType slotType, PersonageModel personage)
        {
            if (!CanDress(asset, slotType))
            {
                UnityEngine.Debug.LogError(string.Format("Can't put on item {0} to slot {1}. Their slot types aren't matched!!! Item is destined for following slots {2}", asset.Classname, slotType.ToString(), string.Join(",", asset.OccupiedSlots)));
                return;
            }

            if (G.self.user_inventory.GetItemAmount(asset.Classname) < 1)
            {
                UnityEngine.Debug.LogError(string.Format("Can't put on item {0}. User don't have enough items", asset.Classname));
                return;
            }
            
            if (personage.TryGetConflictSlotItems(asset, out List<SlotItem> conflictItems))
            {
                for (int i = 0; i < conflictItems.Count; i++)
                    TakeOff(conflictItems[i], personage);
            }
                
            G.self.user_inventory.Decrease(asset.Classname);
            personage.PutOnItem(slotType, asset);
        }
    }
}
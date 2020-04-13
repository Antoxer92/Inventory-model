using System;
using UnityEngine;

namespace Game
{
    [Flags]
    public enum SlotType
    {
        ClothesHead     = 0,
        ClothesBody     = 1 << 0,
        ClothesPants    = 1 << 1,
        WeaponLeftArm   = 1 << 2,
        WeaponRightArm  = 1 << 3,

        // combinations
        Any = ClothesHead | ClothesBody | ClothesPants | WeaponLeftArm | WeaponRightArm
    }
    
    public class DressableAsset : Asset
    {
        [Tooltip("It's slot where item is placed.")]
        [SerializeField]
        private SlotType target_slot;
        [Tooltip("It's slots that item takes. Weapon like bow takes 2 slots : left arm slot and right arm slot")]
        [SerializeField]
        private SlotType[] occupied_slots;

        public SlotType TargetSlot { get => target_slot; } 
        public SlotType[] OccupiedSlots { get => occupied_slots; } 
    }
}
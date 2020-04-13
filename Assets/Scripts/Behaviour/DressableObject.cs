using System;
using UnityEngine;

namespace Game
{
    public enum SlotElementPlacement
    {
        ClothesHead,
        ClothesBody,
        ClothesLeftArm,
        ClothesRightArm,
        ClothesPelvis,
        ClothesLeftLeg,
        ClothesRightLeg,
        WeaponLeftArm,
        WeaponRightArm
    }

    [Serializable]
    public class SlotElement
    {
        [SerializeField]
        private GameObject game_object;
        [SerializeField]
        private SlotElementPlacement placement;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Vector3 rotation;

        public SlotElementPlacement Placement { get => placement; }
        public GameObject GameObject { get => game_object; }
        public Vector3 Position { get => position; }
        public Vector3 Rotation { get => rotation; }
    }

    public class DressableObject : MonoBehaviour
    {
        [SerializeField]
        private SlotElement[] elements;
        public SlotElement[] Elements { get => elements; }
    }
}
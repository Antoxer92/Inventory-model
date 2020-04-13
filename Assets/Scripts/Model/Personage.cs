using UnityEngine;

namespace Game
{ 
    [CreateAssetMenu(menuName = "ScriptableObject/Personage", fileName = "Human")]
    public class Personage : ScriptableObject
    {
        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private GameObject game_object;
        [SerializeField]
        private SlotType[] slots;

        public Sprite Sprite { get => sprite; }
        public GameObject GameObject { get => game_object; }
        public SlotType[] Slots { get => slots; }
    }
}
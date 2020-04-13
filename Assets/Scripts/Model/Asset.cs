using UnityEngine;

namespace Game
{
    public enum AssetGroup
    {
        Clothes,
        Weapon,
        Food,
        Other
    }
    
    [CreateAssetMenu(menuName = "ScriptableObject/Asset", fileName = "Asset")]
    public class Asset : ScriptableObject
    {
        [SerializeField]
        private AssetGroup group;
        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private GameObject prefab;
        [SerializeField]

        public string Classname { get => name; }
        public AssetGroup Group { get => group; }
        public Sprite Icon { get => sprite; }
        public GameObject Prefab { get => prefab; }
    }
}
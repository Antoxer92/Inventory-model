using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "ScriptableObject/Window", fileName = "Window")]
    public class Window : ScriptableObject
    {
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private bool unique;

        public GameObject Prefab { get => prefab; }
        public bool Unique { get => unique; }
    }
}

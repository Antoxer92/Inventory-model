using System;
using UnityEngine;

namespace Game
{
    public class SpawnableModel
    {
        private GameObject prefab;
        public GameObject GameObject { get; private set; }
        public bool Instantiated => GameObject != null;
        
        public SpawnableModel(GameObject prefab)
        {
            this.prefab = prefab;
        }
        
        public void Instantiate()
        {
            if (Instantiated)
                return;
            
            GameObject = MonoBehaviour.Instantiate(prefab);
            AfterInstantiateActions();
        }

        public void Disembody()
        {
            if (!Instantiated)
                return;
            
            BeforeDisembodyActions();
            MonoBehaviour.Destroy(GameObject);
            GameObject = null;
        }

        protected virtual void AfterInstantiateActions() {}

        protected virtual void BeforeDisembodyActions() {}
    }
}
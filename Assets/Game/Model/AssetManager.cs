using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{   
    public class ResourceManager<T> where T : ScriptableObject
    {
        private Type type;
        private string source_directory => type.Name;
        private Dictionary<string, T> resources_map;
        
        public ResourceManager() 
        {
            Init();
        }

        private void Init()
        {
            type = typeof(T);
            resources_map = new Dictionary<string, T>();
            T[] resources = Resources.LoadAll<T>(source_directory);

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i] == null)
                {
                    UnityEngine.Debug.LogError(string.Format("Folder \"Resources\\{0}\" contents resource which isn't instance of type \"{1}\"", source_directory, type.Name));
                    continue;
                }  
                
                if (resources_map.ContainsKey(resources[i].name))
                {
                    UnityEngine.Debug.LogError(string.Format("Dublicate resource with type \"{0}\" and name \"{1}\" detected. Check folder \"Resources\\{2}\"!", type.Name, resources[i].name, source_directory));
                    continue;
                }

                resources_map.Add(resources[i].name, resources[i]);
            }   
        }

        public T GetResource(string classname)
        {
            if (!resources_map.TryGetValue(classname, out T value))
            {
                UnityEngine.Debug.LogError(string.Format("Can't find resource of type \"{0}\" by name \"{1}\"", type.Name, classname));
                return null;
            }
                 
            return (T)value;
        }
    }
}
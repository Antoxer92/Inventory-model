using System.Collections.Generic;

namespace Game
{
    public interface IPoolItem
    {
        string Name { get; }
    }
    
    public class Pool<T> where T : IPoolItem
    {
        private Dictionary<string, Stack<T>> windows;
        
        internal Pool()
        {
            Init();
        }

        private void Init()
        {
            windows = new Dictionary<string, Stack<T>>();
        }

        internal bool TryDequeue(string name, out T window)
        {
            if (windows.TryGetValue(name, out Stack<T> windowStack) && windowStack.Count > 0)
            {
                window = windowStack.Pop();
                return true;
            }

            window = default(T);
            return false;
        }

        internal void Enqueue(T window)
        {
            if (!windows.TryGetValue(window.Name, out Stack<T> windowList))
            {
                windowList = new Stack<T>();
                windows.Add(window.Name, windowList);
            }
                
            windowList.Push(window);
        }
    }
}
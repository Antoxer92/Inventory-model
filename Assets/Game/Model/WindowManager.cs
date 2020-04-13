using System.Collections.Generic;

namespace Game
{
    public class WindowManager
    {
        public delegate void WindowInitializer<T>(T window) where T : BaseWindow;
        
        private ResourceManager<Window> window_resource_manager;
        private List<BaseWindow> active_windows;
        private Pool<BaseWindow> window_pool;

        public WindowManager()
        {
            Init();
        }

        private void Init()
        {
            window_resource_manager = new ResourceManager<Window>();
            active_windows = new List<BaseWindow>();
            window_pool = new Pool<BaseWindow>();
        }

        public void OpenWindow<T>(WindowInitializer<T> init = null) where T : BaseWindow
        {
            var type = typeof(T);

            if (!window_pool.TryDequeue(type.Name, out BaseWindow window))
                window = BaseWindow.Create<T>(window_resource_manager.GetResource(type.Name));

            if (window.Unique)
                CloseWindows(window.Name);
            
            init?.Invoke((T)window);
            active_windows.Add(window);
            window.Open();
        }

        private void CloseWindows(string name)
        {
            for (int i = 0; i < active_windows.Count; i++)
            {
                if (active_windows[i].Name == name)
                    CloseWindow(active_windows[i]);
            }
        }

        public void CloseWindow(BaseWindow window)
        {
            window.Close();
            active_windows.Remove(window);
            window_pool.Enqueue(window);
        }
    }
}
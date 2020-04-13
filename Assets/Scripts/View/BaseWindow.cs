using Game;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour, IPoolItem
{    
    protected Window spec { get; private set; }
    public bool Unique => spec.Unique;
    public string Name => spec.name;

    internal void AttachSpec(Window spec)
    {
        this.spec = spec;
    }
    
    internal void Open()
    {
        gameObject.SetActive(true);
        SelfOpen();
    }

    protected virtual void SelfOpen()
    {
    }

    internal void Close()
    {
        gameObject.SetActive(false);
        SelfClose();
    }

    protected virtual void SelfClose()
    {   
    }

    internal static T Create<T>(Window spec) where T : BaseWindow
    {
        GameObject gameObject = MonoBehaviour.Instantiate(spec.Prefab);
        gameObject.SetActive(false);
        T window = gameObject.GetComponent<T>();
        window.AttachSpec(spec);
        return window;
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup _canvas_group;
    private Transform _transform;
    private Image _image;
    private AnimatedMover _mover;

    private IItemHolder _item_holder;
    private Transform _original_transform;
    private Transform _drag_transform;

    private bool _is_drag = false;

    public IItemHolder ItemHolder => _item_holder;

    void Awake()
    {
        _canvas_group = GetComponent<CanvasGroup>();
        _transform = GetComponent<Transform>();
        _image = GetComponent<Image>();
        _mover = GetComponent<AnimatedMover>();
    }

    internal void Init(IItemHolder itemHolder, Transform originalTransform, Transform dragTransform)
    {
        _item_holder = itemHolder;
        _original_transform = originalTransform;
        _drag_transform = dragTransform;
        _mover.OnComplete += OnComeBack;
        UpdateView();
    }

    internal void UpdateView()
    {
        if (_item_holder.Asset != null)
        {
            _image.sprite = _item_holder.Asset.Icon;
            gameObject.SetActive(true);
        }
        else
        {
            _image.sprite = null;
            gameObject.SetActive(false);
        }
    }

    internal void Clear()
    {
        Deactivate();
        _mover.OnComplete -= OnComeBack;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_item_holder.Asset == null)
            return;

        _is_drag = true;
        Activate();
    }

    private void Activate()
    {
        _transform.SetParent(_drag_transform);
        _canvas_group.alpha = 0.6f;
        _canvas_group.blocksRaycasts = false;
        _item_holder.OnBeginDrag();
    }

    private void Deactivate()
    {
        _canvas_group.blocksRaycasts = true;
        _canvas_group.alpha = 0.0f;
        _transform.SetParent(_original_transform);
        _transform.localPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.position = new Vector3(eventData.position.x, eventData.position.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag();
    }

    public void OnEndDrag(ItemReceiver itemReceiver = null)
    {
        if (!_is_drag)
            return;

        _is_drag = false;
        _item_holder.OnEndDrag(itemReceiver);
        
        if (itemReceiver != null)
            Deactivate();
        else
        {
            _canvas_group.blocksRaycasts = true;
            _mover.Move(_drag_transform.InverseTransformPoint(_original_transform.position), true, 0.25f);
        }  
    }

    private void OnComeBack()
    {
        Deactivate();
    }
}
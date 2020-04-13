using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IItemHolder
{
    DressableAsset Asset { get; }
    PersonageModel Personage { get; }

    void OnBeginDrag();
    void OnEndDrag(ItemReceiver itemReceiver);
}

public class InventoryItemView : MonoBehaviour, IItemHolder
{
    [SerializeField]
    private Image border_img;
    [SerializeField]
    private Image icon_img;
    [SerializeField]
    private TextMeshProUGUI amount_tf;
    [SerializeField]
    private DragableInventoryItem dragable_item;

    [Header("Colors")]
    [SerializeField]
    private Color idle_color = Color.grey;
    [SerializeField]
    private Color capture_color = Color.yellow;
    [SerializeField]
    private Color match_color = Color.green;
    [SerializeField]
    private Color dismatch_color = Color.red;

    private RectTransform _rect_transform;
    private Transform _transform;
    private AnimatedMover _mover;
    private InventoryItemModel _inventory_item;
    private PersonageModel _personage_model;

    public DressableAsset Asset => _inventory_item.Asset as DressableAsset;
    public PersonageModel Personage => _personage_model;

    void Awake()
    {
        _rect_transform = GetComponent<RectTransform>();
        _transform = GetComponent<Transform>();
        _mover = GetComponent<AnimatedMover>();
    }

    void Start()
    {
        border_img.color = idle_color;
    }

    internal void Init(InventoryItemModel inventoryItem, PersonageModel personageModel, Transform dragTransform)
    {
        _inventory_item = inventoryItem;
        _personage_model = personageModel;
        _inventory_item.OnIncrease += OnIncrease;
        _inventory_item.OnDecrease += OnDecrease;

        icon_img.sprite = _inventory_item.Asset.Icon;
        icon_img.enabled = true;
        amount_tf.text = _inventory_item.Amount.ToString();

        dragable_item.Init(this, _transform, dragTransform);

        gameObject.SetActive(true);
    }

    public bool IsMatch(InventoryItemModel inventoryItem)
    {
        return _inventory_item == inventoryItem;
    }

    private void OnDecrease(InventoryItemModel item, int amount, int delta)
    {
        amount_tf.text = amount.ToString();
    }

    private void OnIncrease(InventoryItemModel item, int amount, int delta)
    {
        amount_tf.text = amount.ToString();
    }

    internal void Clear()
    {
        _mover.Stop();
        icon_img.sprite = null;
        icon_img.enabled = false;
        
        _inventory_item.OnIncrease -= OnIncrease;
        _inventory_item.OnDecrease -= OnDecrease;
        _inventory_item = null;
        dragable_item.Clear();

        gameObject.SetActive(false);
    }

    internal Rect GetRect()
    {
        return new Rect(_mover.localPosition - new Vector3(_rect_transform.rect.width/2, -_rect_transform.rect.height/2, 0), _rect_transform.rect.size);
    }

    internal void SetPos(Vector2 pos, bool with_tween)
    {
        var new_pos = new Vector3(pos.x + _rect_transform.rect.width/2, pos.y - _rect_transform.rect.height/2, 0);
        _mover.Move(new_pos, with_tween, 0.25f);
    }

    public void OnBeginDrag()
    {
        border_img.color = capture_color;
    }

    public void OnEndDrag(ItemReceiver itemReceiver)
    {
        border_img.color = idle_color;
    }
}
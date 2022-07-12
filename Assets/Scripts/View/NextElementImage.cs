using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GridUtils;

public class NextElementImage : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    Sprite[] elementSprites;

    [SerializeField]
    Image elementImage;

    UnityEvent<int> onClicked;
    int index;

    public void Init()
    {
        onClicked = new UnityEvent<int>();
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SubscribeToClick(UnityAction<int> action)
    {
        onClicked.AddListener(action);
    }
    public void CallClickedEvent()
    {
        onClicked.Invoke(index);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CallClickedEvent();
    }
    public void Render(GridElement element)
    {
        SetSprite((int)element);
    }

    public void SetSprite(int sprite)
    {
        elementImage.color = new Color(1, 1, 1, elementSprites[sprite] == null ? 0 : 1);
        elementImage.sprite = elementSprites[sprite];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

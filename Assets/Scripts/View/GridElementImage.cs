using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridElementImage : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Sprite[] elementSprites;

    UnityEvent<int, int> onClicked;

    int x, y;

    public void Init()
    {
        onClicked = new UnityEvent<int, int>();
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SubscribeToClick(UnityAction<int, int> action)
    {
        onClicked.AddListener(action);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CallClickedEvent();
    }

    public void CallClickedEvent()
    {
        onClicked.Invoke(x, y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            CallClickedEvent();
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void Render(GeneratorGridElement element, bool transparent = false)
    {
        if (element == null)
        {
            SetSprite(0, transparent);
        }
        else
        {
            SetSprite(element.GetSpriteID(), transparent);
        }
    }


    public void SetSprite(int sprite, bool transparent = false)
    {
        GetComponent<Image>().color = new Color(1, 1, 1, elementSprites[sprite] == null ? 0 : transparent ? 0.5f : 1);
        GetComponent<Image>().sprite = elementSprites[sprite];
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

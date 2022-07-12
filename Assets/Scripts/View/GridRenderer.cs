using UnityEngine;
using UnityEngine.Events;

public class GridRenderer : MonoBehaviour
{
    [SerializeField]
    GameObject gridElementPrefab;

    [SerializeField]
    NextElementImage[] nextElements;

    [SerializeField]
    GameObject floorBar;

    public Vector3 floorBarInitPos;

    GridElementImage[,] gridElements;

    public UnityEvent<int, int> onGridClicked;
    public UnityEvent<int> onNextClicked;

    public int yOffset;

    public void Init()
    {
        onGridClicked = new UnityEvent<int, int>();
        onNextClicked = new UnityEvent<int>();
        gridElements = new GridElementImage[5, 10];
        var rectTransform = GetComponent<RectTransform>();

        int danghostSpace = (int)(Screen.width / 1920f * 90f);
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                gridElements[x, y] = Instantiate(gridElementPrefab, transform).GetComponent<GridElementImage>();
                gridElements[x, y].Init();
                gridElements[x, y].SetPosition(x, y);
                gridElements[x, y].SubscribeToClick((x, y) => onGridClicked.Invoke(x, y));
                gridElements[x, y].gameObject.transform.position = new Vector3(x + 0.5f, y + 0.5f + x % 2 * 0.5f, 0) * danghostSpace + transform.position;
                gridElements[x, y].name = $"{x}, {y}";
            }
        }
        for (int index = 0; index < 12; index++)
        {
            nextElements[index].Init();
            nextElements[index].SubscribeToClick((index) => onNextClicked.Invoke(index));
            nextElements[index].SetIndex(index);
        }
        yOffset = 0;
        floorBarInitPos = floorBar.transform.position;
    }

    public void SubscribeToGridClicked(UnityAction<int, int> action)
    {
        onGridClicked.AddListener(action);
    }

    public void SubscribeToNextClicked(UnityAction<int> action)
    {
        onNextClicked.AddListener(action);
    }

    public void Render(GridData data)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                gridElements[x, y].Render(data.GetElementAt(x, y - yOffset), y - yOffset < 0 || y - yOffset > 9);
            }
        }

        for (int index = 0; index < 12; index++)
        {
            nextElements[index].Render(data.GetNext(index));
        }
        floorBar.transform.localPosition = new Vector3(0, yOffset * 90, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

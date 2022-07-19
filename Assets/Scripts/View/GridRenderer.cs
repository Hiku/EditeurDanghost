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

    [SerializeField]
    GameObject halfHex;

    public Vector3 floorBarInitPos;

    GridElementImage[,] gridElements;

    public UnityEvent<int, int> onGridClicked;
    public UnityEvent<int> onNextClicked;

    public int yOffset;
    public int width, height;
    public int nextAmount;

    public void Init(int width = 5, int height = 10, int nextAmount = 12)
    {
        this.width = width;
        this.height = height;
        this.nextAmount = nextAmount;
        onGridClicked = new UnityEvent<int, int>();
        onNextClicked = new UnityEvent<int>();
        gridElements = new GridElementImage[width, height];
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(90 * width, 90 * (height + 0.5f));

        //int danghostSpace = (int)(Screen.width / 1920f * 90f);
        int danghostSpace = 90;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridElements[x, y] = Instantiate(gridElementPrefab, transform).GetComponent<GridElementImage>();
                gridElements[x, y].Init();
                gridElements[x, y].SetPosition(x, y);
                gridElements[x, y].SubscribeToClick((x, y) => onGridClicked.Invoke(x, y));
                gridElements[x, y].gameObject.transform.localPosition = new Vector3(x + 0.5f - width/2f, y + 0.5f + x % 2 * 0.5f - height - 0.5f, 0) * danghostSpace;
                gridElements[x, y].name = $"{x}, {y}";
            }
        }
        for (int index = 0; index < nextAmount; index++)
        {
            nextElements[index].Init();
            nextElements[index].SubscribeToClick((index) => onNextClicked.Invoke(index));
            nextElements[index].SetIndex(index);
        }
        for(int x = 0; x < width; x++)
        {
            GameObject halfHexInstance = Instantiate(halfHex, transform);
            halfHexInstance.transform.localPosition = new Vector3((x - (width-1)/2f) * danghostSpace, danghostSpace * ((height) * ((x+1) % 2) - height - 0.25f));
            halfHexInstance.transform.eulerAngles = new Vector3(0, 0, (x+1)%2*180);
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

    public void Render(GeneratorGridData data)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridElements[x, y].Render(data.GetElementAt(x, y - yOffset), y - yOffset < 0 || y - yOffset >= height);
            }
        }

        for (int index = 0; index < nextAmount; index++)
        {
            nextElements[index].Render(data.GetNext(index));
        }
        floorBar.transform.localPosition = new Vector3(0, (yOffset + (10-height)) * 90, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

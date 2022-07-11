using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GridRenderer gridRenderer;
    [SerializeField]
    PaintSelector paintSelector;
    [SerializeField]
    InputFieldUpdater inputUpdater;
    GridData currentGridData;


    // Start is called before the first frame update
    void Start()
    {
        currentGridData = new GridData();
        //currentPaint = GridData.GridElement.DANGHOST_BLUE;
        //currentGridData.SetElementAt(4, 3, GridData.GridElement.BOTTLE_CYAN);
        //Debug.Log(gridData.GetElementAt(4, 3));

        gridRenderer.Init();
        gridRenderer.Render(currentGridData);
        gridRenderer.SubscribeToGridClicked(OnGridClicked);
        gridRenderer.SubscribeToNextClicked(OnNextClicked);
    }

    public void OnNextClicked(int index)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            currentGridData.SetNext(index, paintSelector.GetCurrentPaint());
        }
        else if (Input.GetMouseButton(1))
        {
            currentGridData.SetNext(index, GridData.GridElement.EMPTY);
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(currentGridData.GetNext(index));
        }
        gridRenderer.Render(currentGridData);
        inputUpdater.UpdateFromGridData(currentGridData);
    }

    public void OnGridClicked(int x, int y)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            currentGridData.SetElementAt(x, y, paintSelector.GetCurrentPaint());
        }
        else if (Input.GetMouseButton(1))
        {
            currentGridData.SetElementAt(x, y, GridData.GridElement.EMPTY);
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(currentGridData.GetElementAt(x, y));
        }
        gridRenderer.Render(currentGridData);
        inputUpdater.UpdateFromGridData(currentGridData);

        //Debug.Log($"{x} ; {y}");
    }


    public void OnNextClicked(int x, int y)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            currentGridData.SetElementAt(x, y, paintSelector.GetCurrentPaint());
        }
        else if (Input.GetMouseButton(1))
        {
            currentGridData.SetElementAt(x, y, GridData.GridElement.EMPTY);
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(currentGridData.GetElementAt(x, y));
        }
        gridRenderer.Render(currentGridData);

        //Debug.Log($"{x} ; {y}");
    }

    public void OnStepButtonClicked()
    {
        currentGridData.DoOnePopStep();
        gridRenderer.Render(currentGridData);
    }
    public void OnAllStepsButtonClicked()
    {
        currentGridData.DoAllPopSteps();
        gridRenderer.Render(currentGridData);
    }


    // Update is called once per frame
    void Update()
    {

    }
}

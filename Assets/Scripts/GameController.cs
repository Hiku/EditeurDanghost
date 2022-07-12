using System.Collections.Generic;
using UnityEngine;
using static GridUtils;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GridRenderer gridRenderer;
    [SerializeField]
    PaintSelector paintSelector;
    [SerializeField]
    InputFieldUpdater inputUpdater;
    List<GridData> history;
    int currentHistory;


    // Start is called before the first frame update
    void Start()
    {
        //currentPaint = GridData.GridElement.DANGHOST_BLUE;
        //currentGridData.SetElementAt(4, 3, GridData.GridElement.BOTTLE_CYAN);
        //Debug.Log(gridData.GetElementAt(4, 3));
        history = new List<GridData>
        {
            new GridData()
        };
        currentHistory = 0;
        gridRenderer.Init();
        gridRenderer.Render(GetCurrentGridData());
        gridRenderer.SubscribeToGridClicked(OnGridClicked);
        gridRenderer.SubscribeToNextClicked(OnNextClicked);
    }

    public GridData GetCurrentGridData()
    {
        if (history == null) Debug.Log("a");
        return history[currentHistory];
    }

    public void OnNextClicked(int index)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            GetCurrentGridData().SetNext(index, paintSelector.GetCurrentPaint());
        }
        else if (Input.GetMouseButton(1))
        {
            GetCurrentGridData().SetNext(index, GridElement.EMPTY);
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(GetCurrentGridData().GetNext(index));
        }
        gridRenderer.Render(GetCurrentGridData());
        inputUpdater.UpdateFromGridData(GetCurrentGridData());
    }

    public void OnGridClicked(int x, int y)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            if (GetCurrentGridData().GetElementAt(x, y) != paintSelector.GetCurrentPaint())
            {
                AddCurrentToHistory();
                GetCurrentGridData().SetElementAt(x, y, paintSelector.GetCurrentPaint());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (GetCurrentGridData().GetElementAt(x, y) != GridElement.EMPTY)
            {
                AddCurrentToHistory();
                GetCurrentGridData().SetElementAt(x, y, GridElement.EMPTY);
            }
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(GetCurrentGridData().GetElementAt(x, y));
        }
        gridRenderer.Render(GetCurrentGridData());
        inputUpdater.UpdateFromGridData(GetCurrentGridData());
    }

    public void AddCurrentToHistory()
    {
        AddToHistory(GetCurrentGridData().Clone());
    }
    public void AddToHistory(GridData gridData)
    {
        if(currentHistory != history.Count - 1)
            history.RemoveRange(currentHistory + 1, history.Count - currentHistory - 1);
        history.Add(gridData);
        currentHistory = history.Count - 1;

    }

    public void OnStepButtonClicked()
    {
        GridData newGD = GetCurrentGridData().Clone();
        if (newGD.DoOnePopStep())
        {
            AddToHistory(newGD);
        }
        gridRenderer.Render(GetCurrentGridData());
    }
    public void OnAllStepsButtonClicked()
    {
        GridData newGD = GetCurrentGridData().Clone();
        if (newGD.DoAllPopSteps())
        {
            AddToHistory(newGD);
        }
        gridRenderer.Render(GetCurrentGridData());
    }

    public void Cancel()
    {
        
        if (currentHistory > 0)
        {
            currentHistory--;
            gridRenderer.Render(GetCurrentGridData());

        }
    }

    public void Redo()
    {
        if(currentHistory < history.Count - 1)
        {
            currentHistory++;
            gridRenderer.Render(GetCurrentGridData());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            Cancel();
        }
        if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
        {
            Redo();
        }
    }
}

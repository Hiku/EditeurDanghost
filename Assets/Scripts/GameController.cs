using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GridUtils;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GridRenderer gridRenderer;
    [SerializeField]
    PaintSelector paintSelector;
    [SerializeField]
    InputFieldUpdater inputUpdater;
    [SerializeField]
    Image stepButtonImage;
    [SerializeField]
    Image allStepsButtonImage;
    [SerializeField]
    Image scoreResetImage;

    [SerializeField]
    Image undoButtonImage;
    [SerializeField]
    Image redoButtonImage;
    [SerializeField]
    Image inserButtonImage;
    [SerializeField]
    Image allClearButtonImage;

    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    Image upButtonImage;
    [SerializeField]
    Image downButtonImage;
    [SerializeField]
    Image applyOffsetButtonImage;

    List<GridData> history;
    int currentHistory;
    bool insert;


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
        insert = false;
        currentHistory = 0;
        gridRenderer.Init();
        UpdateEditorElements();
        gridRenderer.SubscribeToGridClicked(OnGridClicked);
        gridRenderer.SubscribeToNextClicked(OnNextClicked);
    }

    public GridData GetCurrentGridData()
    {
        return history[currentHistory];
    }


    public void UpdateButtonGrayed()
    {
        Color opaque = Color.white;
        Color transparent = new Color(1, 1, 1, 0.2f);

        bool shouldPopOrFall = GetCurrentGridData().ShouldFall() || GetCurrentGridData().ShouldPop();
        stepButtonImage.color = shouldPopOrFall ? opaque : transparent;
        allStepsButtonImage.color = shouldPopOrFall ? opaque : transparent;

        undoButtonImage.color = currentHistory > 0 ? opaque : transparent;
        redoButtonImage.color = currentHistory < history.Count - 1 ? opaque : transparent;

        inserButtonImage.color = insert ? Color.green : Color.white;

        allClearButtonImage.color = GetCurrentGridData().IsEmpty() ? transparent : opaque;

        scoreResetImage.color = GetCurrentGridData().GetScore() > 0 ? opaque : transparent;

        applyOffsetButtonImage.color = gridRenderer.yOffset != 0 ? opaque : transparent;
    }

    

    public void UpdateEditorElements()
    {
        gridRenderer.Render(GetCurrentGridData());
        inputUpdater.UpdateFromGridData(GetCurrentGridData());

        UpdateButtonGrayed();
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = $"x{GetCurrentGridData().GetMultiplier()}\nScore: {GetCurrentGridData().GetScore()}";
    }

    public void OnNextClicked(int index)
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            if (insert || GetCurrentGridData().GetNext(index) != paintSelector.GetCurrentPaint())
            {
                AddCurrentToHistory();
                if (insert) GetCurrentGridData().PushAllNextElements(index);
                GetCurrentGridData().SetNext(index, paintSelector.GetCurrentPaint());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (insert || GetCurrentGridData().GetNext(index) != GridElement.EMPTY)
            {
                AddCurrentToHistory();
                if (insert) GetCurrentGridData().PullAllNextElements(index);
                else
                    GetCurrentGridData().SetNext(index, GridElement.EMPTY);
            }
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(GetCurrentGridData().GetNext(index));
        }
        UpdateEditorElements();
    }

    public void AllClear()
    {
        if (GetCurrentGridData().IsEmpty()) return;
        AddCurrentToHistory();
        history[history.Count - 1] = new GridData();
        UpdateEditorElements();
    }

    public void OnGridClicked(int x, int y)
    {
        y -= gridRenderer.yOffset;
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButton(0) && !ctrl)
        {
            if (insert || GetCurrentGridData().GetElementAt(x, y) != paintSelector.GetCurrentPaint())
            {
                AddCurrentToHistory();
                if (insert) GetCurrentGridData().PushAllGridElements(x, y);
                GetCurrentGridData().SetElementAt(x, y, paintSelector.GetCurrentPaint());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (insert || GetCurrentGridData().GetElementAt(x, y) != GridElement.EMPTY)
            {
                AddCurrentToHistory();
                if (insert) GetCurrentGridData().PullAllGridElements(x, y);
                else GetCurrentGridData().SetElementAt(x, y, GridElement.EMPTY);
            }
        }
        else if (Input.GetMouseButton(2) || ctrl)
        {
            paintSelector.SelectPaint(GetCurrentGridData().GetElementAt(x, y));
        }
        UpdateEditorElements();
    }

    public void AddCurrentToHistory()
    {
        AddToHistory(GetCurrentGridData().Clone());
    }
    public void AddToHistory(GridData gridData)
    {
        if (currentHistory != history.Count - 1)
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
        UpdateEditorElements();
    }
    public void OnAllStepsButtonClicked()
    {
        GridData newGD = GetCurrentGridData().Clone();
        if (newGD.DoAllPopSteps())
        {
            AddToHistory(newGD);
        }
        UpdateEditorElements();
    }

    public void Cancel()
    {

        if (currentHistory > 0)
        {
            currentHistory--;
            UpdateEditorElements();

        }
    }

    public void RiseOffset()
    {
        gridRenderer.yOffset++;
        UpdateEditorElements();
    }

    public void LowerOffset()
    {
        gridRenderer.yOffset--;
        UpdateEditorElements();
    }

    public void ApplyOffset()
    {
        if(gridRenderer.yOffset != 0)
        {
            AddCurrentToHistory();
            GetCurrentGridData().AddToFloorHeight(-gridRenderer.yOffset);
            gridRenderer.yOffset = 0;
            UpdateEditorElements();

        }
    }

    public void Redo()
    {
        if (currentHistory < history.Count - 1)
        {
            currentHistory++;
            UpdateEditorElements();
        }
    }
    public void ResetScore()
    {
        if (GetCurrentGridData().GetScore() > 0)
        {
            AddCurrentToHistory();
            GetCurrentGridData().ResetScore();
            UpdateEditorElements();
        }
    }

    public void ChangeInsert()
    {
        insert = !insert;
        UpdateEditorElements();
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

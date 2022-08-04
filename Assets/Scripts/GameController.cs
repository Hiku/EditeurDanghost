using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GridRenderer gridRenderer;
    [SerializeField]
    PaintSelector paintSelector;
    [SerializeField]
    InputFieldUpdater gridInputUpdater;
    [SerializeField]
    InputFieldUpdater growInputUpdater;
    [SerializeField]
    InputFieldUpdater nextInputUpdater;
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

    List<GeneratorGridData> history;
    int currentHistory;
    bool insert;
    int height;
    int width;
    int nextAmount;

    // Start is called before the first frame update
    void Start()
    {
        height = GeneratorGridUtils.height;
        width = GeneratorGridUtils.width;
        nextAmount = 12;
        history = new List<GeneratorGridData>
        {
            new GeneratorGridData()
        };

        insert = false;
        currentHistory = 0;
        gridRenderer.Init(width, height);

        UpdateEditorElements();
        gridRenderer.SubscribeToGridClicked(OnGridClicked);
        gridRenderer.SubscribeToNextClicked(OnNextClicked);

    }

    public GeneratorGridData GetCurrentGridData()
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
        gridInputUpdater.UpdateFromGridData(GetCurrentGridData());
        growInputUpdater.UpdateFromGrowData(GetCurrentGridData());
        nextInputUpdater.UpdateFromNextData(GetCurrentGridData());

        gridRenderer.Render(GetCurrentGridData());

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
                if (insert)
                {
                    GetCurrentGridData().PushAllNextElements(index);
                }

                GetCurrentGridData().SetNext(index, paintSelector.GetCurrentPaint());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (insert || GetCurrentGridData().GetNext(index) != GeneratorGridElement.EMPTY)
            {
                AddCurrentToHistory();
                if (insert)
                {
                    GetCurrentGridData().PullAllNextElements(index);
                }
                else
                {
                    GetCurrentGridData().SetNext(index, GeneratorGridElement.EMPTY);
                }
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
        if (GetCurrentGridData().IsEmpty())
        {
            return;
        }

        AddCurrentToHistory();
        history[history.Count - 1] = new GeneratorGridData();
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
                if (insert)
                {
                    GetCurrentGridData().PushAllGridElements(x, y);
                }

                GetCurrentGridData().SetElementAt(x, y, paintSelector.GetCurrentPaint());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (insert || GetCurrentGridData().GetElementAt(x, y) != GeneratorGridElement.EMPTY)
            {
                AddCurrentToHistory();
                if (insert)
                {
                    GetCurrentGridData().PullAllGridElements(x, y);
                }
                else
                {
                    GetCurrentGridData().SetElementAt(x, y, GeneratorGridElement.EMPTY);
                }
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
    public void AddToHistory(GeneratorGridData gridData)
    {
        if (currentHistory != history.Count - 1)
        {
            history.RemoveRange(currentHistory + 1, history.Count - currentHistory - 1);
        }

        history.Add(gridData);
        currentHistory = history.Count - 1;
    }

    public void OnStepButtonClicked()
    {
        GeneratorGridData newGD = GetCurrentGridData().Clone();
        if (newGD.DoOnePopStep())
        {
            AddToHistory(newGD);
        }
        UpdateEditorElements();
    }
    public void OnAllStepsButtonClicked()
    {
        GeneratorGridData newGD = GetCurrentGridData().Clone();
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
        if (gridRenderer.yOffset != 0)
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

    public void UpdateGridFromInputFieldText()
    {
        AddCurrentToHistory();
        gridInputUpdater.FillWithPrefillGridText(GetCurrentGridData());
        UpdateEditorElements();
    }
    public void UpdateGrowFromInputFieldText()
    {
        AddCurrentToHistory();
        growInputUpdater.FillWithGrowGridText(GetCurrentGridData());
        UpdateEditorElements();
    }
    public void UpdateNextFromInputFieldText()
    {
        AddCurrentToHistory();
        nextInputUpdater.FillWithNextText(GetCurrentGridData());
        UpdateEditorElements();
    }
    float difficulty = 3.0f;
    public void TryToFindAllClearModifications()
    {
        // Creates a generator with its basic parameters, and gets a puzzle out of it
        PuzzleGenerator generator = new PuzzleGenerator();
        PuzzleDifficultySetter pds = new PuzzleDifficultySetter(PuzzleGenerator.Power.NONE, difficulty);

        //Debug.Log(difficulty);
        //difficulty += 0.5f;
        pds.ApplyTo(generator);
        GeneratorGridData newGrid = generator.Generate();

        if (newGrid != null)
        {
            AddCurrentToHistory();
            history[history.Count - 1] = newGrid;
            UpdateEditorElements();
        }
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
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
    public void TestButton()
    {
        AddCurrentToHistory();
        // Pouvoir Tatana
        /*
        if (GetCurrentGridData().GetAmountOf(GridElement.TATANA_CUT) > 0)
            GetCurrentGridData().Replace(GridElement.TATANA_CUT, GridElement.EMPTY);
        else
            GetCurrentGridData().TatanaCut();
        */

        // Pouvoir Gravitak

        //GetCurrentGridData().SetGravityReversed(!GetCurrentGridData().IsGravityReversed());
        //GetCurrentGridData().Fall();

        // Pouvoir Kukupin
        //GetCurrentGridData().PlaceKukupinWall(Random.Range(0, 5));


        // Generer des arcs de mode histoire
        /*StoryGenerator.Generate(PuzzleGenerator.Power.NONE);
        StoryGenerator.Generate(PuzzleGenerator.Power.KUKUPIN_WALL);
        StoryGenerator.Generate(PuzzleGenerator.Power.GRAVITAK_GRAVITY);
        StoryGenerator.Generate(PuzzleGenerator.Power.TATANA_CUT);
        */

        // Pouvoir Tutut

        //GetCurrentGridData().SetElementAt(0, 0, new GeneratorGridElement(ElementType.DANGHOST, GridElementColor.BLUE, GridElementColor.RED, 0));

        // Pouvoir Barbak
        //GetCurrentGridData().SetMinGroupSizeToPop(4);
        PuzzleGenerator.Solve(GetCurrentGridData(), false, PuzzleGenerator.Power.GRAVITAK_GRAVITY, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount);
        //Debug.Log(feasibleWithLessPieces + " ; " + feasibleWithoutPower);
        if (bestSolution != null)
        {
            history[history.Count - 1] = bestSolution;
        }

        //Debug.Log("non " + PuzzleGenerator.nonTimes);
        //Debug.Log("oui " + PuzzleGenerator.ouiTimes);

        /*List<int> trucs = new List<int>();
        for(int i = 0; i<50000; i++)
        {
            trucs.Add(Random.Range(int.MinValue, int.MaxValue));
        }
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        int amount = 0;
        for(int i = 0; i<50000; i++)
        {
            if (trucs.Contains(i)) amount++;
        }

        stopWatch.Stop();
        Debug.Log(stopWatch.ElapsedMilliseconds);
        Debug.Log(amount);*/


        //Debug.Log(PuzzleGenerator.IsBeatableWithoutUsingAllPiecesOrPower(GetCurrentGridData(), false, PuzzleGenerator.Power.TUTUT_BICOLOR_PIECE));
        UpdateEditorElements();

    }

}

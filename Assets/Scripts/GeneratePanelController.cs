using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleGeneratorSettings;

public class GeneratePanelController : MonoBehaviour
{
    [SerializeField]
    public PuzzleGeneratorSettings settings;
    public float difficulty;

    [SerializeField]
    public Image loadingBar;

    [SerializeField]
    public GameController gameController;

    [SerializeField]
    public TMP_Dropdown powerDropdown;

    [SerializeField]
    public TMP_Text difficultyText;
    [SerializeField]
    public Slider difficultySlider;
    [SerializeField]
    public TMP_Text colorAmountText;
    [SerializeField]
    public Slider colorAmountSlider;
    [SerializeField]
    public TMP_Text minHeightText;
    [SerializeField]
    public Slider minHeightSlider;
    [SerializeField]
    public TMP_Text maxHeightText;
    [SerializeField]
    public Slider maxHeightSlider;
    [SerializeField]
    public TMP_Text minTicksText;
    [SerializeField]
    public Slider minTicksSlider;
    [SerializeField]
    public TMP_Text ticksGoalText;
    [SerializeField]
    public Slider ticksGoalSlider;
    [SerializeField]
    public TMP_Text ticksGoalImportanceText;
    [SerializeField]
    public Slider ticksGoalImportanceSlider;
    [SerializeField]
    public Toggle ticksGoalNecessaryToggle;
    [SerializeField]
    public TMP_Text oneBottlePerTickImportanceText;
    [SerializeField]
    public Slider oneBottlePerTickImportanceSlider;
    [SerializeField]
    public Toggle oneBottlePerTickNecessaryToggle;
    [SerializeField]
    public TMP_Text noGhostsImportanceText;
    [SerializeField]
    public Slider noGhostsImportanceSlider;
    [SerializeField]
    public Toggle noGhostsNecessaryToggle;
    [SerializeField]
    public TMP_Text clearingImportanceText;
    [SerializeField]
    public Slider clearingImportanceSlider;
    [SerializeField]
    public Toggle clearingNecessaryToggle;
    [SerializeField]
    public TMP_Text bottlesOnTopImportanceText;
    [SerializeField]
    public Slider bottlesOnTopImportanceSlider;
    [SerializeField]
    public Toggle bottlesOnTopNecessaryToggle;
    [SerializeField]
    public TMP_Text nextPieceAmountText;
    [SerializeField]
    public Slider nextPieceAmountSlider;
    [SerializeField]
    public Toggle playableWithoutHoldToggle;
    [SerializeField]
    public Toggle beatableWithOnlyBottlesToggle;
    [SerializeField]
    public TMP_Text enhancementAmountText;
    [SerializeField]
    public Slider enhancementAmountSlider;
    [SerializeField]
    public TMP_Text triesPerEnhancementText;
    [SerializeField]
    public Slider triesPerEnhancementSlider;
    [SerializeField]
    public TMP_Text maxCalculationTimeText;
    [SerializeField]
    public Slider maxCalculationTimeSlider;

    [SerializeField]
    public TMP_Text generateButtonText;

    private bool isUpdatingFromSettings;

    private readonly bool useMaxProgress = false;
    private int[] progress;
    private int maxProgress;
    private bool isStopped;

    public void Start()
    {
        isStopped = true;
        progress = new int[10];
        settings = new PuzzleGeneratorSettings();
        UpdateFromPowerAndDifficulty();
        //UpdateUIFromSettings();
    }

    public void UpdateFromPowerAndDifficulty()
    {
        PuzzleDifficultySetter pds = new PuzzleDifficultySetter(settings.power, difficulty);
        pds.ApplyTo(settings);
        UpdateUIFromSettings();
    }

    public void UpdateUIFromSettings()
    {
        isUpdatingFromSettings = true;
        powerDropdown.value = (int)settings.power;
        difficultyText.text = $"Difficulty: {ShortVer(difficulty)}";
        difficultySlider.value = difficulty;
        colorAmountText.text = $"Color amount: {settings.colorAmount}";
        colorAmountSlider.value = settings.colorAmount;
        minHeightText.text = $"Min height: {ShortVer(settings.minHeight)}";
        minHeightSlider.value = settings.minHeight;
        maxHeightText.text = $"Max height: {ShortVer(settings.maxHeight)}";
        maxHeightSlider.value = settings.maxHeight;
        minTicksText.text = $"Min ticks: {settings.minimumTicks}";
        minTicksSlider.value = settings.minimumTicks;
        ticksGoalText.text = $"Ticks goal: {settings.ticksGoal}";
        ticksGoalSlider.value = settings.ticksGoal;
        ticksGoalImportanceText.text = $"Ticks goal importance: {ShortVer(settings.importanceOfTicksGoal)}";
        ticksGoalImportanceSlider.value = settings.importanceOfTicksGoal;
        ticksGoalNecessaryToggle.isOn = settings.isRespectingTicksGoalNecessary;
        oneBottlePerTickImportanceText.text = $"1 bottle per tick importance: {ShortVer(settings.importanceOf1BottlePerChain)}";
        oneBottlePerTickImportanceSlider.value = settings.importanceOf1BottlePerChain;
        oneBottlePerTickNecessaryToggle.isOn = settings.isHaving1BottlePerChainNecessary;
        noGhostsImportanceText.text = $"No ghosts importance: {ShortVer(settings.importanceOfNotHavingGhosts)}";
        noGhostsImportanceSlider.value = settings.importanceOfNotHavingGhosts;
        noGhostsNecessaryToggle.isOn = settings.isNotHavingGhostsNecessary;
        clearingImportanceText.text = $"Clearing importance: {ShortVer(settings.importanceOfClearing)}";
        clearingImportanceSlider.value = settings.importanceOfClearing;
        clearingNecessaryToggle.isOn = settings.isClearingNecessary;
        bottlesOnTopImportanceText.text = $"Bottle on top importance: {ShortVer(settings.importanceOfHighBottles)}";
        bottlesOnTopImportanceSlider.value = settings.importanceOfHighBottles;
        bottlesOnTopNecessaryToggle.isOn = settings.isHavingAllBottlesOnTopNecessary;
        nextPieceAmountText.text = $"Next piece amount: {settings.nextPieceAmount}";
        nextPieceAmountSlider.value = settings.nextPieceAmount;
        playableWithoutHoldToggle.isOn = settings.shouldBePlayableWithoutHold;
        beatableWithOnlyBottlesToggle.isOn = settings.shouldNextPiecesBeOnlyBottles;
        enhancementAmountText.text = $"Enhancement amount: {settings.enhancementAmount}";
        enhancementAmountSlider.value = settings.enhancementAmount;
        triesPerEnhancementText.text = $"Tries per enhancement: {settings.retriesForEachEnhancement}";
        triesPerEnhancementSlider.value = settings.retriesForEachEnhancement;
        maxCalculationTimeText.text = $"Max calculation time: {ShortVer(settings.computationTime)}s";
        maxCalculationTimeSlider.value = Mathf.Pow(settings.computationTime, 1 / 5f);
        isUpdatingFromSettings = false;

    }

    public void Solve()
    {
        gameController.Solve(settings.power);
    }

    public void Update()
    {
        generateButtonText.text = isStopped ? "Generate" : "Stop";
        if (!isStopped)
        {
            int totalProgress = 0;
            for (int i = 0; i < progress.Length; i++)
            {
                totalProgress += progress[i];
            }
            if (totalProgress > maxProgress)
            {
                maxProgress = totalProgress;
            }
            loadingBar.transform.localScale = new Vector3((useMaxProgress?maxProgress:totalProgress) / (progress.Length * 100f), 1);

        }
        else
        {
            loadingBar.transform.localScale = new Vector3(1, 1);
        }
    }

    public async void Generate()
    {
        if (!isStopped)
        {
            isStopped = true;
            return;
        }
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        maxProgress = 0;
        List<Task<GeneratorGridData>> tasks = new List<Task<GeneratorGridData>>();
        isStopped = false;

        for (int i = 0; i < 10; i++)
        {
            progress[i] = 0;
            int seed = Random.Range(int.MinValue, int.MaxValue);

            int a = i;
            tasks.Add(new Task<GeneratorGridData>(() => GenerateMonothread(ref progress[a], seed, ref isStopped)));
            tasks[i].Start();
        }

        Task<GeneratorGridData> task = await Task.WhenAny(tasks.ToArray());
        isStopped = true;

        //stopUpdatingBar = true;
        //Debug.Log($"took {progress} tries to find a solution");
        GeneratorGridData grid = task.Result;
        if (grid != null)
        {
            gameController.SetAsCurrentGrid(grid);
        }
        await Task.WhenAll(tasks.ToArray());

        /*PuzzleGenerator generator = new PuzzleGenerator(settings);
        GeneratorGridData grid = generator.Generate();
        if(grid != null)
        {
            gameController.SetAsCurrentGrid(grid);
        }*/
    }

    public GeneratorGridData GenerateMonothread(ref int progress, int seed, ref bool stop)
    {
        PuzzleGenerator generator = new PuzzleGenerator(settings);
        GeneratorGridData grid = generator.Generate(ref progress, seed, ref stop);
        return grid;
        //Debug.Log("ouah");
    }

    public void UpdateSettingsFromUI()
    {
        if (isUpdatingFromSettings)
        {
            return;
        }
        settings.power = (Power)powerDropdown.value;

        difficulty = difficultySlider.value;
        settings.colorAmount = (int)colorAmountSlider.value;
        settings.minHeight = minHeightSlider.value;
        settings.maxHeight = maxHeightSlider.value;
        settings.minimumTicks = (int)minTicksSlider.value;
        settings.ticksGoal = (int)ticksGoalSlider.value;
        settings.importanceOfTicksGoal = ticksGoalImportanceSlider.value;
        settings.isRespectingTicksGoalNecessary = ticksGoalNecessaryToggle.isOn;
        settings.importanceOf1BottlePerChain = oneBottlePerTickImportanceSlider.value;
        settings.isHaving1BottlePerChainNecessary = oneBottlePerTickNecessaryToggle.isOn;

        settings.importanceOfNotHavingGhosts = noGhostsImportanceSlider.value;
        settings.isNotHavingGhostsNecessary = noGhostsNecessaryToggle.isOn;

        settings.importanceOfClearing = clearingImportanceSlider.value;
        settings.isClearingNecessary = clearingNecessaryToggle.isOn;

        settings.importanceOfHighBottles = bottlesOnTopImportanceSlider.value;
        settings.isHavingAllBottlesOnTopNecessary = bottlesOnTopNecessaryToggle.isOn;
        settings.nextPieceAmount = (int)nextPieceAmountSlider.value;

        settings.shouldBePlayableWithoutHold = playableWithoutHoldToggle.isOn;
        settings.shouldNextPiecesBeOnlyBottles = beatableWithOnlyBottlesToggle.isOn;

        settings.enhancementAmount = (int)enhancementAmountSlider.value;
        settings.retriesForEachEnhancement = (int)triesPerEnhancementSlider.value;
        settings.computationTime = Mathf.Pow(maxCalculationTimeSlider.value, 5f);
    }

    private string ShortVer(float f)
    {
        string s = f.ToString();
        if (s.Length > 4)
        {
            return s.Substring(0, 4);
        }
        else
        {
            return s;
        }
    }
}

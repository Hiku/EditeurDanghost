using System.Collections.Generic;
using UnityEngine;
using static GeneratorGridUtils;

public class PuzzleGenerator
{
    public enum Power
    {
        NONE,
        TATANA_CUT,
        KUKUPIN_WALL,
        GRAVITAK_GRAVITY,
        MOMONI_CLEAN_COLOR,
        TUTUT_BICOLOR_PIECE,
        GU_ARMOR,
        KUSHINAK_STICKY,
        BARBAK_GROUP3
    }

    public float maxHeight;
    public float minHeight;
    public int colorAmount;
    public int ticksGoal;
    public float importanceOfTicksGoal;
    public float importanceOf1BottlePerChain;
    public float importanceOfHighBottles;
    public float importanceOfNotHavingGhosts;
    public float importanceOfClearing;
    public bool isClearingNecessary;
    public bool isNotHavingGhostsNecessary;
    public bool isHavingAllBottlesOnTopNecessary;
    public bool isHaving1BottlePerChainNecessary;
    public bool isRespectingTicksGoalNecessary;
    public int minimumTicks;
    public int nextPieceAmount;
    public bool shouldBePlayableWithoutHold;
    public bool shouldNextPiecesBeOnlyBottles;
    public int retriesForEachEnhancement;
    public int enhancementAmount;
    public Power power;

    private int kukupinWallColumn;
    private GeneratorGridElement momoniColor;

    private GeneratorGridData addedElements;

    private int[] tatanaNextPosX;
    private int[] tatanaNextPosY;
    private int totalTries = 0;
    public PuzzleGenerator()
    {
        maxHeight = 6;
        minHeight = 4;
        colorAmount = 3;
        ticksGoal = 2;
        importanceOfTicksGoal = 0f;
        importanceOf1BottlePerChain = 0.5f;
        importanceOfHighBottles = 1f;
        importanceOfNotHavingGhosts = 0f;
        importanceOfClearing = 1f;
        isClearingNecessary = true;
        isNotHavingGhostsNecessary = false;
        isHavingAllBottlesOnTopNecessary = false;
        isHaving1BottlePerChainNecessary = false;
        isRespectingTicksGoalNecessary = false;
        nextPieceAmount = 4;
        shouldBePlayableWithoutHold = true;
        shouldNextPiecesBeOnlyBottles = false;
        retriesForEachEnhancement = 100;
        enhancementAmount = 100;
        power = Power.KUKUPIN_WALL;
    }

    // Generates a puzzle based on the parameters
    public GeneratorGridData Generate()
    {
        totalTries++;
        if (totalTries > 3)
        {
            return null;
        }

        if (power == Power.KUKUPIN_WALL)
        {
            kukupinWallColumn = Random.Range(0, width);
        }

        if (power == Power.MOMONI_CLEAN_COLOR)
        {
            momoniColor = GeneratorGridElement.Danghost((GridElementColor)Random.Range(0, colorAmount));
        }

        if (power == Power.TATANA_CUT)
        {
            int[] tatanaHigherColumns = new int[width];
            List<int> tatanaNextPosXList = new List<int>();
            List<int> tatanaNextPosYList = new List<int>();
            for (int i = 0; i < nextPieceAmount; i++)
            {
                int x = Random.Range(0, width);
                tatanaNextPosXList.Add(x);
                tatanaNextPosYList.Add(tatanaHigherColumns[x] + FirstYAboveTatanaCut(x));
                tatanaHigherColumns[x]++;
            }
            tatanaNextPosX = tatanaNextPosXList.ToArray();
            tatanaNextPosY = tatanaNextPosYList.ToArray();
        }

        // First, decides of the heights for each column
        int[] maxHeights = new int[width];
        for (int i = 0; i < maxHeights.Length; i++)
        {
            maxHeights[i] = (int)Random.Range(minHeight, maxHeight);
        }
        // Then make a grid with random danghosts
        GeneratorGridData grid = new GeneratorGridData();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y < maxHeights[x])
                {
                    grid.SetElementAt(x, y, GeneratorGridElement.Danghost((GridElementColor)Random.Range(0, colorAmount)));
                }
                else
                {
                    grid.SetElementAt(x, y, GeneratorGridElement.EMPTY);
                }
            }
        }

        if (power == Power.TUTUT_BICOLOR_PIECE)
        {
            addedElements = new GeneratorGridData();
            int bicolorMoment = Random.Range(0, nextPieceAmount - 1);
            for (int i = 0; i < nextPieceAmount; i++)
            {
                grid.SetNext(i, GeneratorGridElement.Danghost((GridElementColor)Random.Range(0, colorAmount)));
            }
            for (int i = 0; i < nextPieceAmount; i++)
            {
                int column = Random.Range(0, width);
                ElementType type = Random.Range(0, 3) == 0 ? ElementType.DANGHOST : ElementType.BOTTLE;
                GridElementColor color1 = (GridElementColor)grid.GetNext(i).GetColor();
                GridElementColor? color2;
                if (i < bicolorMoment)
                {
                    color2 = null;
                }
                else if (i == bicolorMoment)
                {
                    color2 = (GridElementColor)grid.GetNext(i + 1).GetColor();
                }
                else
                {
                    color2 = (GridElementColor)grid.GetNext(i - 1).GetColor();
                }
                GeneratorGridElement element = new GeneratorGridElement(type, color1, color2);
                addedElements.SetElementAt(column, grid.GetColumnHeight(column), element);
            }
            for (int i = 0; i <= bicolorMoment; i++)
            {
                int exchange = Random.Range(0, bicolorMoment + 1);
                GeneratorGridElement temp = grid.GetNext(i);
                grid.SetNext(i, grid.GetNext(exchange));
                grid.SetNext(exchange, temp);
            }
        }

        // And try to apply changes randomly to enhance it
        int tries = enhancementAmount;

        for (int i = 0; i < tries; i++)
        {
            GeneratorGridData foundGrid = EnhanceGrid(grid, retriesForEachEnhancement, true, 0.9f + (i / enhancementAmount) / 10f, maxHeights);
            if (foundGrid != null)
            {
                grid = foundGrid;
            }
        }

        // If the grid doesn't respect every hard conditions, retry
        if (!RespectsParameterConditions(grid))
        {
            Debug.Log("trying again because the chain doesn't fit the parameter hard conditions");
            return Generate();
        }

        GetGenMultipliers(grid, out int score,
            out int ticks,
         out float _,
         out float _,
         out float _,
         out float _,
         out float _,
         out float _);

        // Transforms the chain into a puzzle by placing a few pieces the hand of the user
        int nextPlaceTries = power == Power.GRAVITAK_GRAVITY ? 3 : 50;
        GeneratorGridData nextPlacedGrid = null;
        if (nextPieceAmount > 0 && power != Power.TUTUT_BICOLOR_PIECE)
        {
            while (nextPlacedGrid == null && nextPlaceTries >= 0)
            {
                nextPlaceTries--;
                nextPlacedGrid = PlaceIntoNext(grid, nextPieceAmount);
            }
            if (nextPlacedGrid == null)
            {
                Debug.Log("trying again because there were not enough high bottles to put in next");
                return Generate();
            }
            Solve(nextPlacedGrid, false, power, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount);

            if (feasibleWithLessPieces)
            {
                Debug.Log("trying again because puzzle is feasible with less pieces");
                return Generate();
            }
            else if (feasibleWithoutPower)
            {
                Debug.Log("trying again because puzzle is feasible without power");
                return Generate();

            }
            return nextPlacedGrid;

        }
        else if (nextPieceAmount == 0)
        {
            return grid;
        }
        else
        {
            Solve(grid, false, Power.TUTUT_BICOLOR_PIECE, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount);

            if (!feasible || feasibleWithLessPieces || feasibleWithoutPower)
            {
                Debug.Log("trying again because the puzzle was beatable without using all pieces or power");
                return Generate();
            }
            return grid;
        }

    }

    private bool RespectsParameterConditions(GeneratorGridData grid)
    {
        GetGenMultipliers(grid,
         out int score,
         out int ticks,
         out float powerMultiplier,
         out float ticksGoalMultiplier,
         out float multipleBottlePerChainMultiplier,
         out float bottleHeightMultiplier,
         out float ghostAmountMultiplier,
         out float elementAfterPoppedMultiplier);
        if (ticks < minimumTicks)
        {
            GetGenMultipliers(grid,
             out int _,
             out int _,
             out float _,
             out float _,
             out float _,
             out float _,
             out float _,
             out float _);

            Debug.Log($"ticks are too low : {ticks} < {minimumTicks}");
            return false;
        }
        if (isRespectingTicksGoalNecessary && ticksGoalMultiplier < 1f)
        {
            Debug.Log("chain doesn't respect ticks goal");
            return false;
        }
        if (isHaving1BottlePerChainNecessary && multipleBottlePerChainMultiplier < 1f)
        {
            Debug.Log("chain has more than 1 bottle per tick");
            return false;
        }
        if (isHavingAllBottlesOnTopNecessary && bottleHeightMultiplier < 1f)
        {
            Debug.Log("chain doesn't have all bottles on top");
            return false;
        }
        if (isNotHavingGhostsNecessary && ghostAmountMultiplier < 1f)
        {
            Debug.Log("chain has ghosts");
            return false;
        }
        if (isClearingNecessary && elementAfterPoppedMultiplier < 1f)
        {
            Debug.Log("chain doesn't clear");
            return false;
        }
        if (score == 0 || powerMultiplier == 0)
        {
            return false;
        }
        return true;
    }

    public static void Solve(GeneratorGridData grid, bool canSpeedCombo, Power power, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount)
    {
        feasible = false;
        feasibleWithLessPieces = false;
        feasibleWithoutPower = false;
        bestSolution = null;
        bestScore = 0;
        solutionAmount = 0;
        GeneratorGridData clone = grid.Clone();
        if (power == Power.TATANA_CUT)
        {
            clone.TatanaCut();
        }
        if (power == Power.GRAVITAK_GRAVITY)
        {
            clone.SetGravityReversed(true);
        }
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        List<int> tried = new List<int>();
        SolveRec(clone, canSpeedCombo, power, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
        //feasible = true;


        stopWatch.Stop();
        Debug.Log(stopWatch.ElapsedMilliseconds);

    }
    public static int nonTimes = 0;
    public static int ouiTimes = 0;

    private static void SolveRec(GeneratorGridData grid, bool canSpeedCombo, Power power, ref bool feasible, ref bool feasibleWithLessPieces, ref bool feasibleWithoutPower, ref GeneratorGridData bestSolution, ref int bestScore, ref int solutionAmount, ref List<int> tried)
    {
        int gridHashCode = grid.GetHashCode();
        if (power != Power.NONE)
            gridHashCode *= -1521134295 + 1;
        if (tried.Contains(gridHashCode))
        {
            nonTimes++;
            return;
        }
        else
        {
            tried.Add(gridHashCode);
            ouiTimes++;
        }
        GeneratorGridData popper = grid.Clone();
        if (power == Power.TATANA_CUT && popper.ShouldPop())
        {
            return;
        }
        if (popper.GetNext(0).IsEmpty() && power == Power.TATANA_CUT)
        {
            popper.Replace(GeneratorGridElement.TATANA_CUT, GeneratorGridElement.EMPTY);
            popper.Fall();
        }
        if (popper.ShouldPop())
        {
            popper.DoAllPopSteps();
            if (!canSpeedCombo && !popper.IsEmpty(true))
            {
                return;
            }
            else if (popper.IsEmpty(true))
            {
                feasible = true;
                feasibleWithLessPieces |= !popper.GetNext(0).IsEmpty();
                feasibleWithoutPower |= power == Power.TUTUT_BICOLOR_PIECE || power == Power.MOMONI_CLEAN_COLOR;
                if (popper.GetScore() > bestScore)
                {
                    bestScore = popper.GetScore();
                    bestSolution = grid;
                }
                solutionAmount++;
                return;

            }
        }

        for (int x = 0; x < width; x++)
        {
            if (!grid.GetNext(0).IsEmpty())
            {

                // Placing the first element as danghost
                GeneratorGridData clone = grid.Clone();
                clone.PlaceElement(x, clone.GetNext(0));
                clone.PullAllNextElements(0);
                SolveRec(clone, canSpeedCombo, power, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                // Placing the first element as bottle
                GeneratorGridData clone2 = grid.Clone();
                clone2.PlaceElement(x, clone2.GetNext(0).GetBottleEquivalent());

                clone2.PullAllNextElements(0);
                SolveRec(clone2, canSpeedCombo, power, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
            }
            if (!grid.GetNext(1).IsEmpty())
            {
                // Placing the second element as danghost
                GeneratorGridData clone3 = grid.Clone();
                clone3.PlaceElement(x, clone3.GetNext(1));
                clone3.PullAllNextElements(1);
                SolveRec(clone3, canSpeedCombo, power, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                // Placing the second element as bottle
                GeneratorGridData clone4 = grid.Clone();
                clone4.PlaceElement(x, clone4.GetNext(1).GetBottleEquivalent());

                clone4.PullAllNextElements(1);
                SolveRec(clone4, canSpeedCombo, power, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
            }
        }

        for (int hold = 0; hold < 2; hold++)
        {
            if (power == Power.TUTUT_BICOLOR_PIECE)
            {
                GeneratorGridData clone = grid.Clone();
                if (!clone.GetNext(hold + 1).IsEmpty())
                {
                    for (int i = hold; i < clone.GetNextAmount(); i++)
                    {
                        if (clone.GetNext(i).IsEmpty())
                        {
                            break;
                        }

                        if (i == hold)
                        {
                            clone.SetNext(i, new GeneratorGridElement(ElementType.DANGHOST, clone.GetNext(i).GetColor(), clone.GetNext(i + 1).GetColor()));
                        }
                        else
                        {
                            clone.SetNext(i, new GeneratorGridElement(ElementType.DANGHOST, clone.GetNext(i).GetColor(), clone.GetNext(i - 1).GetColor()));
                        }
                    }
                }
                SolveRec(clone, canSpeedCombo, Power.NONE, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
            }
            if (power == Power.MOMONI_CLEAN_COLOR)
            {
                GeneratorGridData clone = grid.Clone();
                if (!clone.GetNext(hold).IsEmpty())
                {
                    clone.Replace(clone.GetNext(hold).GetColor(), GeneratorGridElement.EMPTY);

                    clone.PullAllNextElements(hold);
                    clone.Fall();
                }

                SolveRec(clone, canSpeedCombo, Power.NONE, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

            }
        }
    }

    private void GetGenMultipliers(GeneratorGridData grid,
        out int score,
        out int ticks,
        out float powerMultiplier,
        out float ticksGoalMultiplier,
        out float multipleBottlePerChainMultiplier,
        out float bottleHeightMultiplier,
        out float ghostAmountMultiplier,
        out float elementAfterPoppedMultiplier)
    {
        GeneratorGridData popper = grid.Clone();
        popper.ResetScore();
        popper.SetGravityReversed(power == Power.GRAVITAK_GRAVITY);
        powerMultiplier = 1;

        if (power == Power.TUTUT_BICOLOR_PIECE)
        {
            if (popper.ShouldPop())
            {
                powerMultiplier = 0;
            }
            else
            {
                powerMultiplier = Mathf.Max(1 - Mathf.Sqrt(popper.GetAmountOf(ElementType.BOTTLE)) / 4f, 0);
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!addedElements.GetElementAt(x, y).IsEmpty())
                    {
                        popper.SetElementAt(x, y, addedElements.GetElementAt(x, y));
                    }
                }
            }
        }

        if (power == Power.GRAVITAK_GRAVITY)
        {
            popper.Fall();
        }

        if (power == Power.MOMONI_CLEAN_COLOR)
        {
            if (popper.ShouldPop())
            {
                powerMultiplier = 0;
            }
            else
            {
                popper.Replace(momoniColor, GeneratorGridElement.EMPTY);
                popper.Fall();
            }
        }

        if (power == Power.BARBAK_GROUP3)
        {
            if (popper.ShouldPop())
            {
                powerMultiplier = 0;
            }
            else
            {
                popper.SetMinGroupSizeToPop(4);
            }
        }

        int multipleBottlePerChainMalus = 0;
        int bottleHeightMalus = 0;
        for (int x = 0; x < width; x++)
        {
            int bottleAmount = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid.GetElementAt(x, y).IsBottle())
                {
                    bottleAmount++;
                }
                else if (grid.GetElementAt(x, y) != GeneratorGridElement.EMPTY)
                {
                    bottleHeightMalus += bottleAmount;
                }
            }
        }

        if (power == Power.TATANA_CUT)
        {
            popper.TatanaCut();
            if (popper.ShouldPop())
            {
                powerMultiplier = 0;
            }

            popper.Replace(GeneratorGridElement.TATANA_CUT, GeneratorGridElement.EMPTY);
        }

        if (power == Power.KUKUPIN_WALL)
        {
            int wallHeight = popper.ContainsHighKukupinWall(kukupinWallColumn);
            if (wallHeight != -1)
            {
                powerMultiplier = 1 + (float)System.Math.Sqrt((popper.GetAmountOf(ElementType.KUKUPIN_WALL) - 1) / 10f);
                powerMultiplier += (popper.GetColumnHeight(kukupinWallColumn) - 1 - wallHeight) / 10f;
            }
            else
            {
                powerMultiplier = 0;
            }
        }
        ticks = 0;
        bool over = false;
        while (!over)
        {
            if (!popper.ShouldFall())
            {
                multipleBottlePerChainMalus += System.Math.Max(popper.FirstBottlesAmount() - 1, 0);
                if (popper.ShouldPop())
                {
                    ticks++;
                }
            }

            if (!popper.DoOnePopStep())
            {
                over = true;
            }
        }
        ticksGoalMultiplier = importanceOfTicksGoal == 0 ? 1 : System.Math.Max(1f - (float)System.Math.Sqrt(System.Math.Abs(ticks - ticksGoal)) / (5f / importanceOfTicksGoal), 0);
        multipleBottlePerChainMultiplier = importanceOf1BottlePerChain == 0 ? 1 : System.Math.Max(1f - multipleBottlePerChainMalus / (7f / importanceOf1BottlePerChain), 0);
        bottleHeightMultiplier = importanceOfHighBottles == 0 ? 1 : System.Math.Max(1f - (float)System.Math.Sqrt(bottleHeightMalus) / (5f / importanceOfHighBottles), 0);
        ghostAmountMultiplier = importanceOfNotHavingGhosts == 0 ? 1 : System.Math.Max(1f - grid.GetAmountOf(ElementType.GHOST) / (40f / importanceOfNotHavingGhosts), 0);
        elementAfterPoppedMultiplier = importanceOfClearing == 0 ? 1 : System.Math.Max(1f - (float)System.Math.Sqrt(50 - popper.GetAmountOf(ElementType.EMPTY) - popper.GetAmountOf(ElementType.KUKUPIN_WALL)) / (10f / importanceOfClearing), 0);
        score = popper.GetScore();
    }
    // Gets the score of a chain based on the parameters
    private int GetGenScore(GeneratorGridData grid)
    {
        GetGenMultipliers(grid, out int score,
            out int ticks,
        out float powerMultiplier,
        out float ticksGoalMultiplier,
        out float multipleBottlePerChainMultiplier,
        out float bottleHeightMultiplier,
        out float ghostAmountMultiplier,
        out float elementAfterPoppedMultiplier);
        return (int)(score * powerMultiplier * ticksGoalMultiplier * multipleBottlePerChainMultiplier * bottleHeightMultiplier * ghostAmountMultiplier * elementAfterPoppedMultiplier);



    }

    // Places the highest danghosts in the "next" pieces (la réserve)
    public GeneratorGridData PlaceIntoNext(GeneratorGridData grid, int amountToPlace = 4)
    {
        GeneratorGridData clone = grid.Clone();
        clone.SetGravityReversed(power == Power.GRAVITAK_GRAVITY);
        // Elements to place in next for each column
        List<GeneratorGridElement>[] elementByColumn = new List<GeneratorGridElement>[width];
        for (int x = 0; x < width; x++)
        {
            elementByColumn[x] = new List<GeneratorGridElement>();
        }

        GeneratorGridElement firstBottle = GeneratorGridElement.EMPTY;
        int firstBottleColumn = -1;
        if (power == Power.GRAVITAK_GRAVITY)
        {
            clone.Fall();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (clone.GetElementAt(x, y).IsBottle() && clone.GetElementAt(x, y + 1) == GeneratorGridElement.EMPTY)
                {

                    //GridElement danghostEquivalent = DanghostEquivalent(clone.GetElementAt(x, y));
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborsYs);
                    for (int i = 0; i < neighborsYs.Length; i++)
                    {
                        GeneratorGridElement neighbor = clone.GetElementAt(neighborXs[i], neighborsYs[i]);
                        if (neighbor.IsDanghost() && neighbor.IsSameColorAs(clone.GetElementAt(x, y)) && (power != Power.TATANA_CUT || IsAboveTatanaCut(neighborXs[i], neighborsYs[i])))
                        {
                            firstBottleColumn = x;
                        }
                    }
                }
            }
        }
        if (firstBottleColumn == -1)
        {
            if (power == Power.MOMONI_CLEAN_COLOR)
            {
                firstBottle = momoniColor;
            }
            else if (power != Power.TATANA_CUT && power != Power.BARBAK_GROUP3)
            {
                Debug.LogWarning("Error: There was no bottle to trigger the combo");
                return null;
            }
        }
        else
        {
            firstBottle = clone.GetElementAt(firstBottleColumn, clone.GetColumnHeight(firstBottleColumn) - 1);
            clone.SetElementAt(firstBottleColumn, clone.GetColumnHeight(firstBottleColumn) - 1, GeneratorGridElement.EMPTY);
            if (power == Power.GRAVITAK_GRAVITY)
            {
                clone.Fall();
                if (clone.ShouldPop())
                {
                    return null;
                }
            }

        }
        int amountInElementsByColumn = amountToPlace - 1;
        int alreadyInElementsByColumn = 0;
        if (power == Power.TATANA_CUT || power == Power.BARBAK_GROUP3)
        {
            amountInElementsByColumn = amountToPlace;
        }
        if (power == Power.KUKUPIN_WALL)
        {
            amountInElementsByColumn -= 1;
            while (clone.GetElementAt(kukupinWallColumn, clone.GetColumnHeight(kukupinWallColumn) - 1) != GeneratorGridElement.KUKUPIN_WALL)
            {
                if (clone.GetElementAt(kukupinWallColumn, clone.GetColumnHeight(kukupinWallColumn) - 1) == GeneratorGridElement.GHOST)
                {
                    Debug.LogWarning("Ghost was above Kukupin wall");
                    return null;
                }
                elementByColumn[kukupinWallColumn].Add(clone.GetElementAt(kukupinWallColumn, clone.GetColumnHeight(kukupinWallColumn) - 1).GetDanghostEquivalent());
                clone.SetElementAt(kukupinWallColumn, clone.GetColumnHeight(kukupinWallColumn) - 1, GeneratorGridElement.EMPTY);
                alreadyInElementsByColumn++;
            }
            if (alreadyInElementsByColumn > amountInElementsByColumn)
            {
                Debug.LogWarning("Too many elements above Kukupin wall");
                return null;
            }
        }

        for (int i = alreadyInElementsByColumn; i < amountInElementsByColumn; i++)
        {
            float[] chances = new float[width];
            float totalChances = 0;
            float averageY = 0;
            for (int x = 0; x < width; x++)
            {
                averageY += (clone.GetColumnHeight(x) - 1) / (float)width;
            }

            for (int x = 0; x < width; x++)
            {
                int y = clone.GetColumnHeight(x) - 1;
                chances[x] = 2;

                if (clone.GetElementAt(x, y).IsBottle())
                {
                    chances[x]++;
                }
                chances[x] += System.Math.Max(0, System.Math.Abs(x - firstBottleColumn));
                chances[x] += y - averageY + (x % 2) * 0.5f;
                if (clone.GetElementAt(x, y).IsDanghost() && clone.GetElementAt(x, y) == clone.GetElementAt(x, y - 1))
                {
                    chances[x]--;
                }
                if (chances[x] < 0)
                {
                    chances[x] = 0;
                }
                if (clone.GetElementAt(x, y) == GeneratorGridElement.EMPTY || clone.GetElementAt(x, y) == GeneratorGridElement.GHOST || clone.GetElementAt(x, y) == GeneratorGridElement.KUKUPIN_WALL)
                {
                    chances[x] = 0;
                }
                if (shouldNextPiecesBeOnlyBottles && clone.GetElementAt(x, y).IsDanghost())
                {
                    chances[x] = 0;
                }
                if (power == Power.TATANA_CUT && !IsAboveTatanaCut(x, y))
                {
                    chances[x] = 0;
                }

                totalChances += chances[x];
            }
            if (totalChances == 0)
            {
                return null;
            }

            float random = Random.Range(0, totalChances);
            for (int x = 0; x < width; x++)
            {
                random -= chances[x];
                if (random < 0)
                {
                    if (power == Power.GRAVITAK_GRAVITY)
                    {
                        // We have to be able to respect the order, so it's as if it was only one column
                        elementByColumn[0].Add(clone.GetElementAt(x, clone.GetColumnHeight(x) - 1).GetDanghostEquivalent());
                    }
                    else
                    {
                        elementByColumn[x].Add(clone.GetElementAt(x, clone.GetColumnHeight(x) - 1).GetDanghostEquivalent());
                    }
                    clone.SetElementAt(x, clone.GetColumnHeight(x) - 1, GeneratorGridElement.EMPTY);
                    if (power == Power.GRAVITAK_GRAVITY)
                    {
                        clone.Fall();
                        if (clone.ShouldPop())
                        {
                            return null;
                        }
                    }
                    break;
                }
            }
        }
        for (int i = 0; i < clone.GetNextAmount(); i++)
        {
            clone.SetNext(i, GeneratorGridElement.EMPTY);
        }
        GeneratorGridElement hold = firstBottle.GetDanghostEquivalent();
        int currentNextIndex = amountToPlace - 1;
        // By reversing the order the algorithm places the pieces in the hand of the player,
        // we can easily randomize the piece order and sometimes force the player to use the hold button.
        while (amountInElementsByColumn > 0)
        {
            // To simplify the code, if the puzzle should be playable without the use of hold, we just use the hold everytime
            if (shouldBePlayableWithoutHold || Random.Range(0, 3) == 0)
            {
                // Hold and Unhold
                if (hold == GeneratorGridElement.EMPTY)
                {
                    // hold
                    int randomElement = Random.Range(0, amountInElementsByColumn);
                    for (int x = 0; x < elementByColumn.Length; x++)
                    {
                        randomElement -= elementByColumn[x].Count;
                        if (randomElement < 0)
                        {
                            hold = elementByColumn[x][0];
                            elementByColumn[x].RemoveAt(0);
                            break;
                        }
                    }
                    if (randomElement >= 0)
                    {
                        Debug.Log("problème twelve. je répète : problème twelve");
                    }

                    amountInElementsByColumn--;
                }
                else
                {
                    // Unhold
                    clone.SetNext(currentNextIndex, hold);
                    currentNextIndex--;
                    hold = GeneratorGridElement.EMPTY;
                }

            }
            else
            {
                // Place directly in next
                int randomElement = Random.Range(0, amountInElementsByColumn);
                for (int x = 0; x < elementByColumn.Length; x++)
                {
                    randomElement -= elementByColumn[x].Count;
                    if (randomElement < 0)
                    {
                        clone.SetNext(currentNextIndex, elementByColumn[x][0]);
                        elementByColumn[x].RemoveAt(0);
                        break;
                    }
                }
                if (randomElement >= 0)
                {
                    Debug.Log("problème twelve. je répète : problème twelve");
                }

                amountInElementsByColumn--;
                currentNextIndex--;
            }
        }
        if (hold != GeneratorGridElement.EMPTY)
        {
            if (currentNextIndex == -1)
            {
                Debug.Log("oh");
            }

            clone.SetNext(currentNextIndex, hold);
        }
        if (power == Power.GRAVITAK_GRAVITY)
        {
            clone.SetGravityReversed(false);
            clone.Fall();
        }
        if (power == Power.KUKUPIN_WALL)
        {
            clone.SetNext(0, GeneratorGridElement.KUKUPIN_WALL);
            int wallHeight = clone.GetColumnHeight(kukupinWallColumn) - 1;
            if (clone.ShouldPop())
            {
                Debug.LogWarning("Clone should pop and it's weird!!");
                return null;
            }
            int randomizeFromY;
            int randomizeToY;
            if (wallHeight + 2 < maxHeight)
            {
                randomizeFromY = 0;
                randomizeToY = 2;
            }
            else
            {
                randomizeFromY = 1;
                randomizeToY = (int)Random.Range(minHeight, maxHeight) - wallHeight;
            }
            for (int y = randomizeFromY; y <= randomizeToY; y++)
            {
                do
                {
                    GeneratorGridElement randomElement;
                    do
                    {
                        GridElementColor randomColor = (GridElementColor)Random.Range(0, colorAmount);
                        if (Random.Range(0, 2) == 0)
                        {
                            randomElement = GeneratorGridElement.Danghost(randomColor);
                        }
                        else
                        {
                            randomElement = GeneratorGridElement.Bottle(randomColor);
                        }
                    } while (clone.GetAmountOf(randomElement.GetColor()) == 0);
                    clone.SetElementAt(kukupinWallColumn, wallHeight + y, randomElement);
                } while (clone.ShouldPop());
            }

        }
        return clone;
    }

    public GeneratorGridData EnhanceGrid(GeneratorGridData baseGrid, int tries, bool bottlesOnTop, float minScoreMultiplier, int[] maxChangeHeights)
    {
        int bestScore = (int)(GetGenScore(baseGrid) * minScoreMultiplier);

        GeneratorGridData foundGrid = null;
        while (tries >= 0)
        {
            tries--;
            GeneratorGridData clone = baseGrid.Clone();
            int changeAmount = Random.Range(1, 10);
            for (int i = 0; i < changeAmount; i++)
            {
                GeneratorGridElement randomElement;
                int randomX;
                int randomY;
                do
                {
                    GridElementColor randomColor = (GridElementColor)Random.Range(0, colorAmount);

                    if (power == Power.KUKUPIN_WALL && Random.Range(0, 10) == 0)
                    {
                        randomElement = GeneratorGridElement.KUKUPIN_WALL;
                    }
                    else if (Random.Range(0, 9) == 0)
                    {
                        randomElement = GeneratorGridElement.GHOST;
                    }
                    else if (Random.Range(0, 3) == 0)
                    {
                        randomElement = GeneratorGridElement.Bottle(randomColor);
                    }
                    else
                    {
                        randomElement = GeneratorGridElement.Danghost(randomColor);
                    }

                    if (power == Power.TATANA_CUT && Random.Range(0, 5) == 0)
                    {
                        int randomTatanaNext = Random.Range(0, tatanaNextPosX.Length);
                        randomX = tatanaNextPosX[randomTatanaNext];
                        randomY = tatanaNextPosY[randomTatanaNext];
                    }
                    else
                    {
                        randomX = Random.Range(0, width);
                        randomY = Random.Range(0, System.Math.Min(baseGrid.GetHighestLine() + 1, maxChangeHeights[randomX]));
                    }
                } while (clone.GetElementAt(randomX, randomY) == randomElement);
                clone.SetElementAt(randomX, randomY, randomElement);
            }
            //if (clone.ShouldFall())
            //    continue;
            if (bottlesOnTop && !clone.AreAllFirstBottlesOnTop())
            {
                continue;
            }

            if (clone.FirstBottlesAmount() > 1)
            {
                continue;
            }

            int score = GetGenScore(clone);


            if (score >= bestScore)
            {
                bestScore = score;
                foundGrid = clone;
            }

        }
        return foundGrid;
    }

}

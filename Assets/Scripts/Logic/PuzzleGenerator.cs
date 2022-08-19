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
        BARBAK_GROUP3,
        YIYIFU_PAINT_BOMB
    }

    // The maximum height (inclusive) of the puzzle grid columns
    public float maxHeight;
    // The minimum height (inclusive) of the puzzle grid columns
    public float minHeight;
    // The amount of colors in the puzzle
    public int colorAmount;
    // The tick amount that the generator tries to satisfy
    public int ticksGoal;

    // Importances are 0: don't try, 1: default importance, and they can be anything above 0
    public float importanceOfTicksGoal;
    // 1 bottle per chain is about not having too many bottles popping at the same time, which might be confusing and look inefficient
    public float importanceOf1BottlePerChain;
    // Having high bottles makes it feel kind of as if you were playing the real game; you mostly put bottles on top of generated danghosts
    public float importanceOfHighBottles;
    // Ghosts can be confusing for beginers, so it might be better to only have only few of them in easy puzzles
    public float importanceOfNotHavingGhosts;
    // Puzzles should be clearable for now
    public float importanceOfClearing;

    // If the conditions set to true are not satisfied, the puzzle will be generated again
    public bool isClearingNecessary;
    public bool isNotHavingGhostsNecessary;
    public bool isHavingAllBottlesOnTopNecessary;
    public bool isHaving1BottlePerChainNecessary;
    public bool isRespectingTicksGoalNecessary;
    // If the tick amount is lower than the minimum, the puzzle will be generated again
    public int minimumTicks;
    // The amount of pieces the player should use for the puzzle
    public int nextPieceAmount;
    // If the puzzle should be playable without hold, the generator will place pieces in next straightforwardly, which is easier for the player
    public bool shouldBePlayableWithoutHold;
    // It might be easier for the player to only have to place pieces as bottles, for the first puzzles
    public bool shouldNextPiecesBeOnlyBottles;
    // How many ways of enhancing the grid will be tried 
    public int retriesForEachEnhancement;
    // How many times the grid will be enhanced
    public int enhancementAmount;
    // The power the player has to deal with in the puzzle
    public Power power;

    // Column where the player should place the wall in kukupin puzzles
    private int kukupinWallColumn;
    // Color that momoni should clear in momoni puzzles
    private GeneratorGridElement momoniColor;
    private GridElementColor yiyifuColor;
    private int yiyifuAmountBeforePower;

    // Currently only used for Tutut. Elements that should be added by the player.
    // The generator will add them to the grid whenever it wants to calculate the grid score
    private GeneratorGridData addedElements;

    // The positions of the pieces above Tatana's cut (the ones added by the player).
    private int[] tatanaNextPosX;
    private int[] tatanaNextPosY;
    private int totalTries = 0;
    public PuzzleGenerator()
    {
        // Default values, should be overwritten by a PuzzleDifficultySetter in most cases
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
        if (totalTries > 30)
        {
            return null;
        }


        if (power == Power.KUKUPIN_WALL)
        {
            // Decides which column the wall should be placed on
            kukupinWallColumn = Random.Range(0, width);
        }

        if (power == Power.MOMONI_CLEAN_COLOR)
        {
            // Decides which color should be cleared by Momoni at the end of the puzzle
            momoniColor = GeneratorGridElement.Danghost((GridElementColor)Random.Range(0, colorAmount));
        }

        if (power == Power.TATANA_CUT)
        {
            // Decides the positions of the pieces above Tatana's cut (the ones added by the player)
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

        if (power == Power.YIYIFU_PAINT_BOMB)
        {
            // If it's Yiyifu, then the color used for the bomb and when it'll be used has to be defined beforehand
            // The elements Yiyifu places after using the bomb have to be in addedElements too
            addedElements = new GeneratorGridData();
            yiyifuAmountBeforePower = Random.Range(0, nextPieceAmount);
            yiyifuColor = (GridElementColor)Random.Range(0, colorAmount);
            grid.SetNext(yiyifuAmountBeforePower, GeneratorGridElement.Danghost(yiyifuColor));

            for (int i = yiyifuAmountBeforePower + 1; i < nextPieceAmount; i++)
            {
                grid.SetNext(i, GeneratorGridElement.Danghost((GridElementColor)Random.Range(0, colorAmount)));
            }
            for (int i = yiyifuAmountBeforePower; i < nextPieceAmount; i++)
            {
                int column = Random.Range(0, width);
                ElementType type = Random.Range(0, 3) == 0 ? ElementType.DANGHOST : ElementType.BOTTLE;
                GridElementColor color = (GridElementColor)grid.GetNext(i).GetColor();
                GeneratorGridElement element = new GeneratorGridElement(type, color);
                addedElements.SetElementAt(column, Mathf.Max(grid.GetColumnHeight(column), addedElements.GetColumnHeight(column)), element);
            }

            if (!shouldBePlayableWithoutHold)
            {
                for (int i = yiyifuAmountBeforePower; i < nextPieceAmount; i++)
                {
                    // Switches pieces randomly in order to make the player use the hold button
                    int exchange = Random.Range(yiyifuAmountBeforePower, nextPieceAmount);
                    GeneratorGridElement temp = grid.GetNext(i);
                    grid.SetNext(i, grid.GetNext(exchange));
                    grid.SetNext(exchange, temp);
                }
            }

        }

        if (power == Power.TUTUT_BICOLOR_PIECE)
        {
            // If it's Tutut, then pieces that the player will use can't be picked directly from the generated grid,
            // because the generated grid doesn't contain bicolor pieces.
            // Thus, the pieces are chosen at the beginning and never changed after.
            // Enhancements of the grid will try to fit with these pieces.
            addedElements = new GeneratorGridData();
            // When the player should use the power.
            int bicolorMoment = Random.Range(0, nextPieceAmount - 2);

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
                addedElements.SetElementAt(column, Mathf.Max(grid.GetColumnHeight(column), addedElements.GetColumnHeight(column)), element);
            }
            if (!shouldBePlayableWithoutHold)
            {
                for (int i = 0; i <= bicolorMoment; i++)
                {
                    // Switches pieces randomly in order to make the player use the hold button
                    int exchange = Random.Range(0, bicolorMoment + 1);
                    GeneratorGridElement temp = grid.GetNext(i);
                    grid.SetNext(i, grid.GetNext(exchange));
                    grid.SetNext(exchange, temp);
                }
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
        if (nextPieceAmount > 0)
        {
            if (power == Power.TUTUT_BICOLOR_PIECE)
                nextPlacedGrid = grid;
            while (nextPlacedGrid == null && nextPlaceTries >= 0)
            {
                nextPlaceTries--;
                if (power == Power.YIYIFU_PAINT_BOMB)
                {
                    nextPlacedGrid = PlaceIntoNext(grid, yiyifuAmountBeforePower);
                }
                else
                {
                    nextPlacedGrid = PlaceIntoNext(grid, nextPieceAmount);
                }
            }
            if (nextPlacedGrid == null)
            {
                Debug.Log("trying again because there were not enough high bottles to put in next");
                return Generate();
            }
            Solve(nextPlacedGrid, false, power, true, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount);

            if (!feasible)
            {
                Debug.Log("trying again because puzzle is not feasible");
                return Generate();

            }
            else if (feasibleWithLessPieces)
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
        else
        {
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

    public static void Solve(GeneratorGridData grid, bool canSpeedCombo, Power power, bool stopOnFound, out bool feasible, out bool feasibleWithLessPieces, out bool feasibleWithoutPower, out GeneratorGridData bestSolution, out int bestScore, out int solutionAmount, bool ignoreFeasibleWithLessPieces = false, bool ignoreFeasibleWithoutPower = false)
    {
        feasible = false;
        feasibleWithLessPieces = ignoreFeasibleWithLessPieces;
        feasibleWithoutPower = ignoreFeasibleWithoutPower;
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
            clone.Fall();
        }
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        Dictionary<int, bool> tried = new Dictionary<int, bool>();
        SolveRec(clone, canSpeedCombo, power, stopOnFound, false, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
        //feasible = true;


        stopWatch.Stop();
        Debug.Log(stopWatch.ElapsedMilliseconds);

    }
    // test only, to remove
    public static int nonTimes = 0;
    public static int ouiTimes = 0;

    // feasibleWithLessPieces and feasibleWithoutPower should be set to true when calling this if you don't need to know about them
    private static void SolveRec(GeneratorGridData grid, bool canSpeedCombo, Power power, bool stopOnFound, bool holdWasUsed, ref bool feasible, ref bool feasibleWithLessPieces, ref bool feasibleWithoutPower, ref GeneratorGridData bestSolution, ref int bestScore, ref int solutionAmount, ref Dictionary<int, bool> tried)
    {
        if (stopOnFound && feasible && feasibleWithLessPieces && feasibleWithoutPower)
        {
            return;
        }
        // The last thing I tried to reduce the amount of calculation; ended up lowering it by about 30%, but still taking too much time imo
        int gridHashCode = grid.GetHashCode();
        if (power != Power.NONE)
        {
            gridHashCode *= -1521134295 + 1;
        }
        // test only, to remove
        if (tried.ContainsKey(gridHashCode))
        {
            nonTimes++;
            return;
        }
        else
        {
            tried.Add(gridHashCode, true);
            ouiTimes++;
        }
        GeneratorGridData popper = grid.Clone();
        bool shouldPop = popper.ShouldPop();
        if (power == Power.TATANA_CUT && shouldPop)
        {
            return;
        }
        if (popper.GetNext(0).IsEmpty() && power == Power.TATANA_CUT)
        {
            popper.Replace(GeneratorGridElement.TATANA_CUT, GeneratorGridElement.EMPTY);
            popper.Fall();
            shouldPop = popper.ShouldPop();
        }
        if (shouldPop)
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
                int score = popper.GetScore();
                if (score > bestScore)
                {
                    bestScore = score;
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
                SolveRec(clone, canSpeedCombo, power, stopOnFound, holdWasUsed, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                // Placing the first element as bottle
                GeneratorGridData clone2 = grid.Clone();
                clone2.PlaceElement(x, clone2.GetNext(0).GetBottleEquivalent());

                clone2.PullAllNextElements(0);
                SolveRec(clone2, canSpeedCombo, power, stopOnFound, holdWasUsed, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
            }
            if (!grid.GetNext(1).IsEmpty())
            {
                // Placing the second element as danghost
                GeneratorGridData clone3 = grid.Clone();
                clone3.PlaceElement(x, clone3.GetNext(1));
                clone3.PullAllNextElements(1);
                SolveRec(clone3, canSpeedCombo, power, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                // Placing the second element as bottle
                GeneratorGridData clone4 = grid.Clone();
                clone4.PlaceElement(x, clone4.GetNext(1).GetBottleEquivalent());

                clone4.PullAllNextElements(1);
                SolveRec(clone4, canSpeedCombo, power, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
            }
        }

        if(power == Power.BARBAK_GROUP3)
        {
            GeneratorGridData clone = grid.Clone();
            clone.SetMinGroupSizeToPop(3);
            SolveRec(clone, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
        }

        for (int currentPiece = 0; currentPiece < 2; currentPiece++)
        {
            if (power == Power.TUTUT_BICOLOR_PIECE)
            {
                if (currentPiece == 0 && !holdWasUsed)
                {
                    // In the case hold wasn't used, the player can use Tutut's power on 1st and 2nd piece.
                    GeneratorGridData clone2 = grid.Clone();
                    if (UseTututPowerForSolve(clone2, 0, 1))
                    {
                        SolveRec(clone2, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
                    }
                }
                // And in general, the merge starts between the current piece and the piece at index 2, because the other is the hold one.
                GeneratorGridData clone = grid.Clone();
                if (UseTututPowerForSolve(clone, currentPiece, 2))
                {
                    SolveRec(clone, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);
                }
            }
            if (power == Power.MOMONI_CLEAN_COLOR)
            {
                GeneratorGridData clone = grid.Clone();
                if (!clone.GetNext(currentPiece).IsEmpty())
                {
                    clone.Replace(clone.GetNext(currentPiece).GetColor(), GeneratorGridElement.EMPTY);

                    clone.PullAllNextElements(currentPiece);
                    clone.Fall();
                }

                SolveRec(clone, canSpeedCombo, Power.NONE, stopOnFound, holdWasUsed, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

            }
            if (power == Power.YIYIFU_PAINT_BOMB)
            {
                GeneratorGridData clone = grid.Clone();
                if (grid.GetNext(currentPiece).IsEmpty())
                {
                    continue;
                }

                GridElementColor color = (GridElementColor)clone.GetNext(currentPiece).GetColor();
                UseYiyifuPowerForSolve(clone, color);
                SolveRec(clone, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                /*for (int column = 0; column < 5; column++)
                {
                    if (currentPiece == 0 && !holdWasUsed)
                    {
                        // In the case hold wasn't used, the player can use Yiyifu's power on 1st and 2nd piece.
                        GeneratorGridData clone2 = grid.Clone();
                        UseYiyifuPowerForSolve(clone2, 0, 1, column);
                        SolveRec(clone2, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                    }
                    // And in general, the merge starts between the current piece and the piece at index 2, because the other is the hold one.
                    GeneratorGridData clone = grid.Clone();
                    UseYiyifuPowerForSolve(clone, currentPiece, 2, column);

                    SolveRec(clone, canSpeedCombo, Power.NONE, stopOnFound, true, ref feasible, ref feasibleWithLessPieces, ref feasibleWithoutPower, ref bestSolution, ref bestScore, ref solutionAmount, ref tried);

                }*/
            }
        }
    }
    private static bool UseTututPowerForSolve(GeneratorGridData grid, int currentPiece, int nextPiecesStart)
    {
        if (grid.GetNext(nextPiecesStart).IsEmpty())
        {
            return false;
        }
        grid.SetNext(currentPiece, new GeneratorGridElement(ElementType.DANGHOST, grid.GetNext(currentPiece).GetColor(), grid.GetNext(nextPiecesStart).GetColor()));
        grid.SetNext(nextPiecesStart, new GeneratorGridElement(ElementType.DANGHOST, grid.GetNext(currentPiece).GetColor(), grid.GetNext(nextPiecesStart).GetColor()));
        for (int i = nextPiecesStart + 1; i < grid.GetNextAmount(); i++)
        {
            if (grid.GetNext(i).IsEmpty())
            {
                break;
            }
            grid.SetNext(i, new GeneratorGridElement(ElementType.DANGHOST, grid.GetNext(i).GetColor(), grid.GetNext(i - 1).GetColor()));
        }
        return true;
    }

    /*private static void UseYiyifuPowerForSolve(GeneratorGridData grid, int currentPiece, int nextPiecesStart, int column)
    {
        List<GridElementColor> colors = new List<GridElementColor>();
        for (int y = height - 1; y >= 0; y--)
        {
            GeneratorGridElement elem = grid.GetElementAt(column, y);
            if (elem.IsDanghost() || elem.IsBottle())
            {

            }
        }
    }*/
    private static void UseYiyifuPowerForSolve(GeneratorGridData grid, GridElementColor color)
    {
        int[] xPriority = new int[] { 2, 1, 3, 0, 4 };
        int bestX = 0;
        int bestY = 0;
        int bestScore = 0;
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                int differentColoredElements = 0;
                GridElementColor? elementColor = grid.GetElementAt(xPriority[x], y).GetColor();
                if (elementColor != null && elementColor != color)
                {
                    differentColoredElements++;
                }

                GetNeighbors(xPriority[x], y, out int[] neighborXs, out int[] neighborYs);
                for (int i = 0; i < neighborXs.Length; i++)
                {
                    GridElementColor? neighborColor = grid.GetElementAt(neighborXs[i], neighborYs[i]).GetColor();
                    if (neighborColor != null && neighborColor != color)
                    {
                        differentColoredElements++;
                    }
                }
                if (differentColoredElements > bestScore)
                {
                    bestScore = differentColoredElements;
                    bestX = xPriority[x];
                    bestY = y;
                }
            }
        }
        if (bestScore > 0)
        {
            if (grid.GetElementAt(bestX, bestY).IsBottle())
            {
                grid.SetElementAt(bestX, bestY, GeneratorGridElement.Bottle(color));
            }

            if (grid.GetElementAt(bestX, bestY).IsDanghost())
            {
                grid.SetElementAt(bestX, bestY, GeneratorGridElement.Danghost(color));
            }

            GetNeighbors(bestX, bestY, out int[] neighborXs, out int[] neighborYs);
            for (int i = 0; i < neighborXs.Length; i++)
            {
                if (grid.GetElementAt(neighborXs[i], neighborYs[i]).IsBottle())
                {
                    grid.SetElementAt(neighborXs[i], neighborYs[i], GeneratorGridElement.Bottle(color));
                }

                if (grid.GetElementAt(neighborXs[i], neighborYs[i]).IsDanghost())
                {
                    grid.SetElementAt(neighborXs[i], neighborYs[i], GeneratorGridElement.Danghost(color));
                }
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

        if (power == Power.YIYIFU_PAINT_BOMB)
        {
            if (popper.ShouldPop())
            {
                powerMultiplier = 0;
            }
            else
            {
                powerMultiplier = 1;
            }
            UseYiyifuPowerForSolve(popper, yiyifuColor);
        }

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
        }
        if (power == Power.TUTUT_BICOLOR_PIECE || power == Power.YIYIFU_PAINT_BOMB)
        {
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
                popper.SetMinGroupSizeToPop(3);
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
            else if (power != Power.TATANA_CUT && power != Power.BARBAK_GROUP3 && power != Power.YIYIFU_PAINT_BOMB)
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
        if (power == Power.TATANA_CUT || power == Power.BARBAK_GROUP3 || power == Power.YIYIFU_PAINT_BOMB)
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
        /*for (int i = 0; i < clone.GetNextAmount(); i++)
        {
            clone.SetNext(i, GeneratorGridElement.EMPTY);
        }*/
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
                        Debug.Log("Weird issue happened");
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

using System.Collections.Generic;
using UnityEngine;
using static GridUtils;

public class PuzzleGenerator
{
    public int maxHeight;
    public int minHeight;
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
    public int nextPieceAmount;
    public bool shouldBePlayableWithoutHold;
    public bool shouldNextPiecesBeOnlyBottles;
    public PuzzleGenerator()
    {
        maxHeight = 6;
        minHeight = 4;
        colorAmount = 6;
        ticksGoal = 6;
        importanceOfTicksGoal = 0f;
        importanceOf1BottlePerChain = 1f;
        importanceOfHighBottles = 1f;
        importanceOfNotHavingGhosts = 0f;
        importanceOfClearing = 1f;
        isClearingNecessary = false;
        isNotHavingGhostsNecessary = false;
        isHavingAllBottlesOnTopNecessary = false;
        isHaving1BottlePerChainNecessary = false;
        isRespectingTicksGoalNecessary = true;
        nextPieceAmount = 4;
        shouldBePlayableWithoutHold = true;
        shouldNextPiecesBeOnlyBottles = true;
    }

    // Generates a puzzle based on the parameters
    public GridData Generate()
    {
        // First, decides of the heights for each column
        int[] maxHeights = new int[5];
        for (int i = 0; i < maxHeights.Length; i++)
        {
            maxHeights[i] = Random.Range(minHeight, maxHeight + 1);
        }
        // Then make a grid with random danghosts
        GridData grid = new GridData();
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (y < maxHeights[x])
                {
                    grid.SetElementAt(x, y, (GridElement)Random.Range(1, 1 + colorAmount));
                }
                else
                {
                    grid.SetElementAt(x, y, GridElement.EMPTY);
                }
            }
        }
        // And try to apply changes randomly to enhance it
        int tries = 150;

        for (int i = 0; i < tries; i++)
        {
            GridData foundGrid = EnhanceGrid(grid, 300, true, 0.9f + (i / tries) / 10f, maxHeights);
            if (foundGrid != null)
            {
                grid = foundGrid;
            }
        }
        // If the grid doesn't respect every hard conditions, retry
        if (!RespectsParameterConditions(grid))
        {
            Debug.Log("trying again");
            return Generate();
        }


        // Score should be the goal of the puzzle
        int score = grid.GetPopScore();
        // For now we use a Debug.Log to show it to the user, should update this
        Debug.Log(score);

        // Transforms the chain into a puzzle by placing a few pieces the hand of the user
        grid = PlaceIntoNext(grid, nextPieceAmount);
        return grid;
    }

    private bool RespectsParameterConditions(GridData grid)
    {
        GridData popper = grid.Clone();
        popper.ResetScore();
        int multipleBottlePerChainMalus = 0;
        int bottleHeightMalus = 0;
        for (int x = 0; x < 5; x++)
        {
            int bottleAmount = 0;
            for (int y = 0; y < 10; y++)
            {
                if (IsBottle(grid.GetElementAt(x, y))) bottleAmount++;
                else if (grid.GetElementAt(x, y) != GridElement.EMPTY) bottleHeightMalus += bottleAmount;
            }
        }
        int ticks = 0;
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
        if (isRespectingTicksGoalNecessary && ticks != ticksGoal)
            return false;
        if (isHaving1BottlePerChainNecessary && multipleBottlePerChainMalus > 0)
            return false;
        if (isHavingAllBottlesOnTopNecessary && bottleHeightMalus > 0)
            return false;
        if (isNotHavingGhostsNecessary && grid.GetAmountOf(GridElement.GHOST) > 0)
            return false;
        if (isClearingNecessary && !popper.IsEmpty())
            return false;
        return true;
    }

    // Gets the score of a chain based on the parameters
    private int GetGenScore(GridData grid)
    {
        GridData popper = grid.Clone();
        popper.ResetScore();
        int multipleBottlePerChainMalus = 0;
        int bottleHeightMalus = 0;
        for (int x = 0; x < 5; x++)
        {
            int bottleAmount = 0;
            for (int y = 0; y < 10; y++)
            {
                if (IsBottle(grid.GetElementAt(x, y))) bottleAmount++;
                else if (grid.GetElementAt(x, y) != GridElement.EMPTY) bottleHeightMalus += bottleAmount;
            }
        }
        int ticks = 0;
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
        float ticksGoalMultiplier = importanceOfTicksGoal == 0?1:System.Math.Max(1f - (float)System.Math.Sqrt(System.Math.Abs(ticks - ticksGoal)) / (5f / importanceOfTicksGoal), 0);
        float multipleBottlePerChainMultiplier = importanceOf1BottlePerChain == 0 ? 1 : System.Math.Max(1f - multipleBottlePerChainMalus / (7f / importanceOf1BottlePerChain), 0);
        float bottleHeightMultiplier = importanceOfHighBottles == 0 ? 1 : System.Math.Max(1f - (float)System.Math.Sqrt(bottleHeightMalus) / (5f / importanceOfHighBottles), 0);
        float ghostAmountMultiplier = importanceOfNotHavingGhosts == 0 ? 1 : System.Math.Max(1f - grid.GetAmountOf(GridElement.GHOST) / (40f / importanceOfNotHavingGhosts), 0);
        float elementAfterPoppedMultiplier = importanceOfClearing == 0 ? 1 : System.Math.Max(1f - (float)System.Math.Sqrt(50 - popper.GetAmountOf(GridElement.EMPTY)) / (10f / importanceOfClearing), 0);
        return (int)(popper.GetScore() * ticksGoalMultiplier * multipleBottlePerChainMultiplier * bottleHeightMultiplier * ghostAmountMultiplier * elementAfterPoppedMultiplier);



    }

    // Places the highest danghosts in the "next" pieces (la réserve)
    public GridData PlaceIntoNext(GridData grid, int amountToPlace = 4)
    {
        GridData clone = grid.Clone();
        // Elements to place in next for each column
        List<GridElement>[] elementByColumn = new List<GridElement>[5];
        for (int x = 0; x < 5; x++)
            elementByColumn[x] = new List<GridElement>();
        GridElement firstBottle;
        int firstBottleColumn = -1;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (IsBottle(clone.GetElementAt(x, y)) && clone.GetElementAt(x, y + 1) == GridElement.EMPTY)
                {
                    GridElement danghostEquivalent = DanghostEquivalent(clone.GetElementAt(x, y));
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborsYs);
                    for (int i = 0; i < neighborsYs.Length; i++)
                    {
                        if (clone.GetElementAt(neighborXs[i], neighborsYs[i]) == danghostEquivalent)
                        {
                            firstBottleColumn = x;
                        }
                    }
                }
            }
        }
        if (firstBottleColumn == -1)
        {
            Debug.LogWarning("Error: There was no bottle to trigger the combo");
            return null;
        }
        else
        {
            firstBottle = clone.GetElementAt(firstBottleColumn, clone.GetColumnHeight(firstBottleColumn) - 1);
            clone.SetElementAt(firstBottleColumn, clone.GetColumnHeight(firstBottleColumn) - 1, GridElement.EMPTY);
        }

        for (int i = 0; i < amountToPlace - 1; i++)
        {
            float[] chances = new float[5];
            float totalChances = 0;
            float averageY = 0;
            for (int x = 0; x < 5; x++)
            {
                averageY += (clone.GetColumnHeight(x) - 1) / 5f;
            }
            for (int x = 0; x < 5; x++)
            {
                int y = clone.GetColumnHeight(x) - 1;
                chances[x] = 2;
                if (IsBottle(clone.GetElementAt(x, y)))
                {
                    chances[x]++;
                }
                chances[x] += System.Math.Max(0, System.Math.Abs(x - firstBottleColumn));
                chances[x] += y - averageY + (x % 2) * 0.5f;
                if (IsDanghost(clone.GetElementAt(x, y)) && clone.GetElementAt(x, y) == clone.GetElementAt(x, y - 1))
                {
                    chances[x]--;
                }
                if (chances[x] < 0)
                {
                    chances[x] = 0;
                }
                if (clone.GetElementAt(x, y) == GridElement.EMPTY || clone.GetElementAt(x, y) == GridElement.GHOST)
                {
                    chances[x] = 0;
                }
                if(shouldNextPiecesBeOnlyBottles && IsDanghost(clone.GetElementAt(x, y)))
                {
                    chances[x] = 0;
                }
                totalChances += chances[x];
            }
            float random = Random.Range(0, totalChances);
            for (int x = 0; x < 5; x++)
            {
                random -= chances[x];
                if (random < 0)
                {

                    elementByColumn[x].Add(DanghostEquivalent(clone.GetElementAt(x, clone.GetColumnHeight(x) - 1)));
                    clone.SetElementAt(x, clone.GetColumnHeight(x) - 1, GridElement.EMPTY);
                    break;
                }
            }
        }
        for (int i = 0; i < 12; i++)
        {
            clone.SetNext(i, GridElement.EMPTY);
        }
        GridElement hold = DanghostEquivalent(firstBottle);
        int currentNextIndex = amountToPlace - 1;
        int amountInElementsByColumn = amountToPlace - 1;
        // By reversing the order the algorithm places the pieces in the hand of the player,
        // we can easily randomize the piece order and sometimes force the player to use the hold button.
        while (amountInElementsByColumn > 0)
        {
            // To simplify the code, if the puzzle should be playable without the use of hold, we just use the hold everytime
            if (shouldBePlayableWithoutHold || Random.Range(0, 3) == 0)
            {
                // Hold and Unhold
                if (hold == GridElement.EMPTY)
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
                    if (randomElement >= 0) Debug.Log("problème twelve. je répète : problème twelve");
                    amountInElementsByColumn--;
                }
                else
                {
                    // Unhold
                    clone.SetNext(currentNextIndex, hold);
                    currentNextIndex--;
                    hold = GridElement.EMPTY;
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
                if (randomElement >= 0) Debug.Log("problème twelve. je répète : problème twelve");
                amountInElementsByColumn--;
                currentNextIndex--;
            }
        }
        if (hold != GridElement.EMPTY)
        {
            clone.SetNext(0, hold);
        }
        return clone;
    }

    public GridData EnhanceGrid(GridData baseGrid, int tries, bool bottlesOnTop, float minScoreMultiplier, int[] maxChangeHeights)
    {
        int bestScore = (int)(GetGenScore(baseGrid) * minScoreMultiplier);

        GridData foundGrid = null;
        while (tries >= 0)
        {
            tries--;
            GridData clone = baseGrid.Clone();
            int changeAmount = Random.Range(1, 10);
            for (int i = 0; i < changeAmount; i++)
            {
                GridElement randomElement;
                int randomX;
                int randomY;
                do
                {
                    if (Random.Range(0, 10) == 0) randomElement = GridElement.EMPTY;
                    else if (Random.Range(0, 9) == 0) randomElement = GridElement.GHOST;
                    else if (Random.Range(0, 3) == 0) randomElement = (GridElement)Random.Range(7, 7 + colorAmount);
                    else randomElement = (GridElement)Random.Range(1, 1 + colorAmount);
                    randomX = Random.Range(0, 5);
                    randomY = Random.Range(0, System.Math.Min(baseGrid.GetHighestLine() + 1, maxChangeHeights[randomX]));
                } while (clone.GetElementAt(randomX, randomY) == randomElement);
                clone.SetElementAt(randomX, randomY, randomElement);
            }
            if (clone.ShouldFall())
                continue;
            if (bottlesOnTop && !clone.AreAllFirstBottlesOnTop())
                continue;
            if (clone.FirstBottlesAmount() > 1)
                continue;
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

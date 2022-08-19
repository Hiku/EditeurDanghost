using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GeneratorGridUtils;

public class GeneratorGridData
{
    private List<GeneratorGridElement[]> grid;
    private GeneratorGridElement[] nexts;
    private int floorHeight;
    private int currentScore;
    private int currentMultiplier;
    private int currentChain;
    private bool gravityReversed;
    private int minGroupSizeToPop;
    public GeneratorGridData()
    {
        grid = new List<GeneratorGridElement[]>();
        nexts = new GeneratorGridElement[12];
        for (int i = 0; i < nexts.Length; i++)
        {
            nexts[i] = GeneratorGridElement.EMPTY;
        }
        currentScore = 0;
        currentMultiplier = 0;
        currentChain = 0;
        gravityReversed = false;
        minGroupSizeToPop = -1;
    }

    public void SetMinGroupSizeToPop(int size)
    {
        minGroupSizeToPop = size;
    }

    public bool IsGravityReversed()
    {
        return gravityReversed;
    }

    public void SetGravityReversed(bool reversed)
    {
        gravityReversed = reversed;
    }

    public void ClearAbove(int y)
    {
        int actualY = y + floorHeight;
        if (actualY < grid.Count)
        {
            grid.RemoveRange(actualY, grid.Count - actualY);
        }
    }
    public int GetNextAmount()
    {
        return nexts.Count();
    }

    public void ClearBelow(int y)
    {
        int actualY = y + floorHeight;
        if (actualY > 0)
        {
            grid.RemoveRange(0, actualY);
        }

        floorHeight -= actualY;
    }

    public GeneratorGridElement GetNext(int index)
    {
        return nexts[index];
    }

    public void AddToFloorHeight(int offset)
    {
        floorHeight += offset;
    }
    public int GetAmountOf(GridElementColor? color)
    {
        int amount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y).GetColor() == color || GetElementAt(x, y).GetColor2() == color)
                {
                    amount++;
                }
            }
        }
        return amount;

    }

    public int GetAmountOf(ElementType type)
    {
        int amount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y).GetElementType() == type)
                {
                    amount++;
                }
            }
        }
        return amount;
    }
    public int GetAmountOf(GeneratorGridElement element)
    {
        int amount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y) == element)
                {
                    amount++;
                }
            }
        }
        return amount;
    }

    public void TatanaCut()
    {
        SetElementAt(0, 3, GeneratorGridElement.TATANA_CUT);
        SetElementAt(0, 5, GeneratorGridElement.TATANA_CUT);
        SetElementAt(1, 3, GeneratorGridElement.TATANA_CUT);
        SetElementAt(1, 4, GeneratorGridElement.TATANA_CUT);
        SetElementAt(2, 4, GeneratorGridElement.TATANA_CUT);
        SetElementAt(3, 3, GeneratorGridElement.TATANA_CUT);
        SetElementAt(3, 4, GeneratorGridElement.TATANA_CUT);
        SetElementAt(4, 3, GeneratorGridElement.TATANA_CUT);
        SetElementAt(4, 5, GeneratorGridElement.TATANA_CUT);
    }


    // Returns true if there's a wall in the highest 3 elements of the column
    public int ContainsHighKukupinWall(int x)
    {
        int brokenElements = 0;
        bool foundOtherWall = false;
        int y;
        for (y = height - 1; y >= 0; y--)
        {
            if (GetElementAt(x, y).IsEmpty())
            {

            }
            else if (GetElementAt(x, y) == GeneratorGridElement.KUKUPIN_WALL)
            {
                foundOtherWall = true;
            }
            else
            {
                brokenElements++;
            }
            if (brokenElements >= 3)
            {
                return -1;
            }
            if (foundOtherWall)
            {
                return y;
            }
        }
        return -1;
    }

    public void PlaceKukupinWall(int x)
    {
        int brokenElements = 0;
        bool foundOtherWall = false;
        for (int y = height - 1; y >= 0; y--)
        {
            if (GetElementAt(x, y).IsEmpty())
            {

            }
            else if (GetElementAt(x, y) == GeneratorGridElement.KUKUPIN_WALL)
            {
                foundOtherWall = true;
            }
            else
            {
                brokenElements++;
                SetElementAt(x, y, GeneratorGridElement.EMPTY);
            }
            if (foundOtherWall || brokenElements == 3)
            {
                break;
            }
        }
        if (!foundOtherWall)
        {
            SetElementAt(x, GetColumnHeight(x), GeneratorGridElement.KUKUPIN_WALL);
        }
    }
    public void SetNext(int index, GeneratorGridElement value)
    {
        nexts[index] = value;
    }

    public int GetPopScore()
    {
        GeneratorGridData clone = Clone();
        clone.DoAllPopSteps();
        return clone.GetScore();
    }
    public void ResetScore()
    {
        currentChain = 0;
        currentMultiplier = 0;
        currentScore = 0;
    }

    public int GetMultiplier()
    {
        return currentMultiplier;
    }
    public int GetScore()
    {
        return currentScore;
    }


    public GeneratorGridElement GetElementAt(int x, int y)
    {
        int actualY = y + floorHeight;
        if (actualY < 0 || actualY >= grid.Count || x < 0 || x >= width)
        {
            return GeneratorGridElement.EMPTY;
        }

        return grid[actualY][x];
    }

    public int GetHighestLine()
    {
        return grid.Count - floorHeight;
    }
    public int GetLowestLine()
    {
        return -floorHeight;
    }

    public void PullAllGridElements(int x, int y)
    {
        int maxY = GetHighestLine();
        for (int i = y; i < maxY; i++)
        {
            SetElementAt(x, i, GetElementAt(x, i + 1));
        }
    }
    public void PushAllGridElements(int x, int y)
    {
        int maxY = GetHighestLine();
        for (int i = maxY; i > y; i--)
        {
            SetElementAt(x, i, GetElementAt(x, i - 1));
        }
        //SetElementAt(x, y, GridElement.EMPTY);
    }

    public void Replace(GeneratorGridElement from, GeneratorGridElement to)
    {
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] == from)
                {
                    grid[y][x] = to;
                }
            }
        }
    }
    public void Replace(GridElementColor? from, GeneratorGridElement to)
    {
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x].GetColor() == from)
                {
                    grid[y][x] = to;
                }
            }
        }
    }

    public void PushAllNextElements(int index)
    {
        for (int i = nexts.Length - 2; i >= index; i--)
        {
            SetNext(i + 1, GetNext(i));
        }
    }

    public void PullAllNextElements(int index)
    {
        for (int i = index; i < nexts.Length - 1; i++)
        {
            SetNext(i, GetNext(i + 1));
        }
        SetNext(nexts.Length - 1, GeneratorGridElement.EMPTY);
    }

    public bool IsEmpty(bool acceptWalls = false)
    {
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] != GeneratorGridElement.EMPTY && (acceptWalls == false || grid[y][x].GetElementType() != ElementType.KUKUPIN_WALL))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void SetElementAt(int x, int y, GeneratorGridElement element)
    {
        int actualY = y + floorHeight;
        while (actualY < 0)
        {
            AddLineBelow();
            actualY = y + floorHeight;
        }
        while (actualY >= grid.Count)
        {
            AddLineAbove();
        }
        grid[actualY][x] = element;
    }

    public void AddLineBelow()
    {
        grid.Insert(0, MakeLine());
        floorHeight++;
    }

    public void AddLineAbove()
    {
        grid.Add(MakeLine());
    }
    private List<GeneratorGridElement[]> GridClone()
    {
        List<GeneratorGridElement[]> clone = new List<GeneratorGridElement[]>();
        for (int i = 0; i < grid.Count; i++)
        {
            clone.Add(MakeLine());
            for (int j = 0; j < grid[i].Length; j++)
            {
                clone[i][j] = grid[i][j];
            }
        }
        return clone;
    }

    public GeneratorGridElement[] NextsClone()
    {
        return nexts.Where(e => true).ToArray();
    }

    public GeneratorGridData Clone()
    {
        GeneratorGridData clone = new GeneratorGridData();
        clone.floorHeight = floorHeight;
        clone.grid = GridClone();
        clone.nexts = NextsClone();
        clone.currentMultiplier = currentMultiplier;
        clone.currentChain = currentChain;
        clone.currentScore = currentScore;
        clone.gravityReversed = gravityReversed;
        clone.minGroupSizeToPop = minGroupSizeToPop;
        //Debug.Log(width);
        return clone;
    }

    public int GetColumnFillAmount(int column)
    {
        int total = 0;
        for (int i = 0; i < height; i++)
        {
            if (GetElementAt(column, i) != GeneratorGridElement.EMPTY)
            {
                total++;
            }
        }
        return total;
    }

    public int GetColumnHeight(int column)
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (GetElementAt(column, i) != GeneratorGridElement.EMPTY)
            {
                return i + 1;
            }
        }
        return 0;
    }
    public int GetColumnHeightReversed(int column)
    {
        for (int i = 0; i < height; i++)
        {
            if (GetElementAt(column, i) != GeneratorGridElement.EMPTY)
            {
                return i - 1;
            }
        }
        return height - 1;

    }

    public int GetColumnHoles(int column)
    {
        if (gravityReversed)
        {
            return (height - GetColumnHeightReversed(column) - 1) - GetColumnFillAmount(column);
        }

        return GetColumnHeight(column) - GetColumnFillAmount(column);
    }

    public bool ShouldFall()
    {
        for (int i = 0; i < width; i++)
        {
            if (GetColumnHoles(i) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool FallUpwards(int column)
    {
        int columnHeight = height - 1;
        bool changed = false;
        for (int i = height - 1; i >= 0; i--)
        {
            if (GetElementAt(column, i) != GeneratorGridElement.EMPTY)
            {
                if (columnHeight != i)
                {
                    SetElementAt(column, columnHeight, GetElementAt(column, i));
                    SetElementAt(column, i, GeneratorGridElement.EMPTY);
                    changed = true;
                }
                columnHeight--;
            }
        }
        return changed;
    }

    public bool Fall(int column)
    {
        if (gravityReversed)
        {
            return FallUpwards(column);
        }

        int columnHeight = 0;
        bool changed = false;
        for (int i = 0; i < height; i++)
        {
            if (GetElementAt(column, i) != GeneratorGridElement.EMPTY)
            {
                if (columnHeight != i)
                {
                    SetElementAt(column, columnHeight, GetElementAt(column, i));
                    SetElementAt(column, i, GeneratorGridElement.EMPTY);
                    changed = true;
                }
                columnHeight++;
            }
        }
        return changed;
    }

    public bool AreAllBottlesOnTop()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y).IsBottle() && (!(GetElementAt(x, y + 1)).IsBottle() && GetElementAt(x, y + 1) != GeneratorGridElement.EMPTY))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool AreAllFirstBottlesOnTop()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y).IsBottle() && GetElementAt(x, y + 1) != GeneratorGridElement.EMPTY)
                {
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborsYs);
                    for (int i = 0; i < neighborsYs.Length; i++)
                    {
                        GeneratorGridElement neighbor = GetElementAt(neighborXs[i], neighborsYs[i]);
                        if (neighbor.IsDanghost() && neighbor.IsSameColorAs(GetElementAt(x, y)))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void PlaceElement(int x, GeneratorGridElement element)
    {
        if (element == GeneratorGridElement.KUKUPIN_WALL)
        {
            PlaceKukupinWall(x);
        }
        else if (gravityReversed)
        {
            for (int i = 0; i < height - 1; i++)
            {
                SetElementAt(x, i, GetElementAt(x, i + 1));
            }
            SetElementAt(x, height - 1, element);

        }
        else
        {
            SetElementAt(x, GetColumnHeight(x), element);
        }
    }

    public int FirstBottlesAmount()
    {
        int amount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y).IsBottle())
                {
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborsYs);
                    for (int i = 0; i < neighborsYs.Length; i++)
                    {
                        GeneratorGridElement neighbor = GetElementAt(neighborXs[i], neighborsYs[i]);

                        if (neighbor.IsDanghost() && neighbor.IsSameColorAs(GetElementAt(x, y)))
                        {
                            amount++;
                        }
                    }
                }
            }
        }
        return amount;

    }


    public bool Fall()
    {
        bool fell = false;
        for (int i = 0; i < width; i++)
        {
            fell |= Fall(i);
        }

        return fell;
    }

    public bool ShouldPop()
    {
        return Clone().Pop();
    }


    public bool Pop()
    {
        // MakeGroupIDs crée des groupes de danghosts liés entre eux, en leur donnant un identifiant commun, sans prendre en compte les bouteilles.
        int[,] groupIDs = MakeGroupIDs();


        // À partir de son identifiant, est-ce que le groupe doit disparaître après cette étape ou non. Initialisé à false.
        bool[] shouldGroupDisappear = new bool[width * height];
        // Pop par group size
        if (minGroupSizeToPop > 0)
        {
            int[] groupCount = new int[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (groupIDs[x, y] != -1)
                    {
                        groupCount[groupIDs[x, y]]++;
                    }
                }
            }

            for (int i = 0; i < groupCount.Length; i++)
            {
                if (groupCount[i] >= minGroupSizeToPop)
                {
                    shouldGroupDisappear[i] = true;
                }
            }
        }

        // Pop par bouteille
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GeneratorGridElement bottle = GetElementAt(x, y);
                // Pour chaque bouteille de la grille
                if (bottle.IsBottle())
                {
                    bool shouldBottleDisappear = false;
                    // Son identifiant est créé de la même manière que celui des danghosts
                    int bottleID = x * height + y;

                    // Pour chaque élément autour de la bouteille
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborYs);
                    for (int i = 0; i < neighborXs.Length; i++)
                    {
                        GeneratorGridElement neighbor = GetElementAt(neighborXs[i], neighborYs[i]);
                        // Si c'est un danghost qui doit connecter avec la bouteille
                        if (neighbor.IsDanghost() && neighbor.IsSameColorAs(bottle))
                        {
                            // On "groupe" la bouteille avec les danghosts
                            // Ce qui va avoir pour effet de fusionner les groupes liés à la bouteille entre eux
                            // Ce qui permet d'éviter de les compter comme des groupes différents dans les calculs de score
                            int neighborID = groupIDs[neighborXs[i], neighborYs[i]];
                            if (neighborID < bottleID)
                            {
                                if (bottleID < x * height + y)
                                {
                                    ReplaceInGroupIDs(groupIDs, neighborID, bottleID);
                                }
                                else
                                {
                                    bottleID = neighborID;
                                }
                            }
                            else
                            {
                                ReplaceInGroupIDs(groupIDs, neighborID, bottleID);
                            }
                            // On dit que la bouteille doit disparaître à la fin
                            shouldBottleDisappear = true;
                        }
                    }
                    if (shouldBottleDisappear)
                    {
                        // On enlève la bouteille
                        SetElementAt(x, y, GeneratorGridElement.EMPTY);

                        // Mais aussi les ghosts autour de la bouteille
                        for (int i = 0; i < neighborXs.Length; i++)
                        {
                            if (GetElementAt(neighborXs[i], neighborYs[i]) == GeneratorGridElement.GHOST)
                            {
                                SetElementAt(neighborXs[i], neighborYs[i], GeneratorGridElement.EMPTY);
                            }
                        }
                    }
                    // Le groupe devra disparaître à la fin.
                    // On le fait plus tard et pas dans cette loop au cas où plusieurs bouteilles y sont connectées
                    shouldGroupDisappear[bottleID] = shouldBottleDisappear;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Pour chaque ghost/echo restant
                if (GetElementAt(x, y) == GeneratorGridElement.GHOST)
                {
                    // S'ils ont des danghosts qui cassent autour d'eux
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborYs);
                    bool shouldGhostDisappear = false;
                    for (int i = 0; i < neighborXs.Length; i++)
                    {
                        if (groupIDs[neighborXs[i], neighborYs[i]] != -1 && shouldGroupDisappear[groupIDs[neighborXs[i], neighborYs[i]]])
                        {
                            shouldGhostDisappear = true;
                            break;
                        }
                    }
                    if (shouldGhostDisappear)
                    {
                        // On les enlève
                        SetElementAt(x, y, GeneratorGridElement.EMPTY);
                    }
                }
            }
        }


        int totalAmount = 0;
        int[] amountByGroup = new int[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Pour chaque danghost qui doit disparaître
                if (groupIDs[x, y] != -1 && shouldGroupDisappear[groupIDs[x, y]])
                {
                    totalAmount++;
                    // On compte le danghost comme faisant partie de ce groupe
                    amountByGroup[groupIDs[x, y]]++;
                    // Et on l'efface.
                    SetElementAt(x, y, GeneratorGridElement.EMPTY);
                }
            }
        }
        bool disappeared = false;
        // Calcul du score : Pour chaque groupe, dans le même ordre que dans le jeu
        for (int i = 0; i < amountByGroup.Length; i++)
        {
            if (shouldGroupDisappear[i])
            {
                if (currentMultiplier > 0)
                {
                    // Si quelque chose a déjà cassé, c'est que c'est une chaîne, donc on augmente le multiplier, et la chaîne
                    currentMultiplier += width + currentChain * 2;
                    currentChain++;
                }
                disappeared = true;
            }
        }
        // Une fois les multipliers augmentés, on augmente le score
        currentScore += (totalAmount * (totalAmount + 1) / 2 + totalAmount * currentMultiplier) * 100;
        // On augmente aussi le multiplier pour chaque danghost qui a cassé
        currentMultiplier += totalAmount;
        //Debug.Log(currentMultiplier);
        //Debug.Log(currentScore);
        return disappeared;
    }
    /// <summary>
    /// Crée les groupes de danghosts, liés entre eux en leur donnant un identifiant commun, sans prendre en compte les bouteilles.
    /// </summary>
    /// <returns>Un array à deux dimensions [x, y] avec l'identifiant de chaque groupe, selon l'ordre dans lequel il devrait casser.</returns>
    public int[,] MakeGroupIDs()
    {
        int[,] groupIDs = MakeGroupIDsBase();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (groupIDs[x, y] != -1)
                {
                    if (GetAbove(x, y, out int xAbove, out int yAbove))
                    {
                        TryToGroup(groupIDs, x, y, xAbove, yAbove);
                    }

                    if (GetUpperRight(x, y, out int xUR, out int yUR))
                    {
                        TryToGroup(groupIDs, x, y, xUR, yUR);
                    }

                    if (GetLowerRight(x, y, out int xLR, out int yLR))
                    {
                        TryToGroup(groupIDs, x, y, xLR, yLR);
                    }
                }
            }
        }
        return groupIDs;
    }

    /// <summary>
    /// Place chaque danghost dans son propre groupe, seul.
    /// </summary>
    /// <returns>Un array à deux dimensions [x, y] avec l'identifiant de chaque danghost, selon l'ordre dans lequel il devrait casser.</returns>
    public int[,] MakeGroupIDsBase()
    {
        int[,] groupIDs = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetElementAt(x, y) == null)
                {
                    Debug.Log("oh what the fuck");
                }
                if (GetElementAt(x, y).IsDanghost())
                {
                    groupIDs[x, y] = x * height + y;
                }
                else
                {
                    groupIDs[x, y] = -1;
                }
            }
        }
        return groupIDs;
    }

    /// <summary>
    /// Essaie de grouper 2 danghosts.
    /// S'ils sont de la même couleur, leurs groupes respectifs sont fusionnés sous le même ID, qui est le minimum entre les deux.
    /// </summary>
    /// <param name="groupIDs">Les identifiants de groupes jusque-là. Sera modifié pour fusionner les groupes si besoin.</param>
    /// <param name="x">La position x du premier danghost.</param>
    /// <param name="y">La position y du premier danghost.</param>
    /// <param name="x2">La position x du deuxième danghost.</param>
    /// <param name="y2">La position y du deuxième danghost.</param>
    public void TryToGroup(int[,] groupIDs, int x, int y, int x2, int y2)
    {
        GeneratorGridElement element1 = GetElementAt(x, y);
        GeneratorGridElement element2 = GetElementAt(x2, y2);

        if (element1.IsDanghost() && element2.IsDanghost() && element1.IsSameColorAs(element2))
        {
            if (groupIDs[x2, y2] < groupIDs[x, y])
            {
                if (groupIDs[x, y] == x * height + y)
                {
                    groupIDs[x, y] = groupIDs[x2, y2];
                }
                else
                {
                    ReplaceInGroupIDs(groupIDs, groupIDs[x, y], groupIDs[x2, y2]);
                }
            }
            else
            {
                if (groupIDs[x2, y2] == x2 * height + y2)
                {
                    groupIDs[x2, y2] = groupIDs[x, y];
                }
                else
                {
                    ReplaceInGroupIDs(groupIDs, groupIDs[x2, y2], groupIDs[x, y]);
                }
            }
        }
    }


    /// <summary>
    /// Change l'identifiant de chaque élément du groupe "from" par "to".
    /// </summary>
    /// <param name="groupIDs"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void ReplaceInGroupIDs(int[,] groupIDs, int from, int to)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (groupIDs[x, y] == from)
                {
                    groupIDs[x, y] = to;
                }
            }
        }
    }

    public bool DoOnePopStep()
    {
        if (Fall())
        {
            return true;
        }

        return Pop();
    }

    public bool DoAllPopSteps()
    {
        int times = 0;
        while (DoOnePopStep() && times++ < 1000)
        {
            ;
        }

        if (times >= 1000)
        {
            Debug.Log("boucle infinie");
        }

        return times != 0;
    }

    public override int GetHashCode()
    {
        int hashCode = -1793996700;
        foreach (GeneratorGridElement[] line in grid)
        {
            foreach(GeneratorGridElement element in line)
            {
                hashCode = hashCode * -1521134295 + element.GetHashCode();
            }
        }
        foreach (GeneratorGridElement element in nexts)
        {
            hashCode = hashCode * -1521134295 + element.GetHashCode();
        }

        hashCode = hashCode * -1521134295 + floorHeight.GetHashCode();
        hashCode = hashCode * -1521134295 + currentScore.GetHashCode();
        hashCode = hashCode * -1521134295 + currentMultiplier.GetHashCode();
        hashCode = hashCode * -1521134295 + currentChain.GetHashCode();
        hashCode = hashCode * -1521134295 + gravityReversed.GetHashCode();
        hashCode = hashCode * -1521134295 + minGroupSizeToPop.GetHashCode();
        return hashCode;
    }
}

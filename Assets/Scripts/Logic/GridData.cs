using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridUtils;

public class GridData
{
    private List<GridElement[]> grid;
    private GridElement[] nexts;
    private int floorHeight;
    private int currentScore;
    private int currentMultiplier;
    private int currentChain;

    public GridData()
    {
        grid = new List<GridElement[]>();
        nexts = new GridElement[12];
        currentScore = 0;
        currentMultiplier = 0;
        currentChain = 0;
    }

    public GridElement GetNext(int index)
    {
        return nexts[index];
    }

    public void AddToFloorHeight(int offset)
    {
        floorHeight += offset;
    }

    public void SetNext(int index, GridElement value)
    {
        nexts[index] = value;
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


    public GridElement GetElementAt(int x, int y)
    {
        int actualY = y + floorHeight;
        if (actualY < 0 || actualY >= grid.Count || x < 0 || x >= 5) return GridElement.EMPTY;
        return grid[actualY][x];
    }

    public int GetHighestLine()
    {
        return grid.Count - floorHeight;
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
        SetNext(nexts.Length - 1, GridElement.EMPTY);
    }

    public bool IsEmpty()
    {
        for (int y = 0; y < grid.Count; y++)
            for (int x = 0; x < grid[y].Length; x++)
                if (grid[y][x] != GridElement.EMPTY)
                    return false;
        return true;
    }

    public void SetElementAt(int x, int y, GridElement element)
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
    private List<GridElement[]> GridClone()
    {
        List<GridElement[]> clone = new List<GridElement[]>();
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

    public GridElement[] NextsClone()
    {
        return nexts.Where(e => true).ToArray();
    }

    public GridData Clone()
    {
        GridData clone = new GridData();
        clone.floorHeight = floorHeight;
        clone.grid = GridClone();
        clone.nexts = NextsClone();
        clone.currentMultiplier = currentMultiplier;
        clone.currentChain = currentChain;
        clone.currentScore = currentScore;
        return clone;
    }

    public int GetColumnFillAmount(int column)
    {
        int total = 0;
        for (int i = 0; i < 10; i++)
        {
            if (GetElementAt(column, i) != GridElement.EMPTY) total++;
        }
        return total;
    }

    public int GetColumnHeight(int column)
    {
        for (int i = 9; i >= 0; i--)
        {
            if (GetElementAt(column, i) != GridElement.EMPTY) return i + 1;
        }
        return 0;
    }

    public int GetColumnHoles(int column)
    {
        return GetColumnHeight(column) - GetColumnFillAmount(column);
    }

    public bool ShouldFall()
    {
        for (int i = 0; i < 5; i++)
        {
            if (GetColumnHoles(i) > 0) return true;
        }
        return false;
    }

    public bool Fall(int column)
    {
        int height = 0;
        bool changed = false;
        for (int i = 0; i < 10; i++)
        {
            if (GetElementAt(column, i) != GridElement.EMPTY)
            {
                if (height != i)
                {
                    SetElementAt(column, height, GetElementAt(column, i));
                    SetElementAt(column, i, GridElement.EMPTY);
                    changed = true;
                }
                height++;
            }
        }
        return changed;
    }

    public bool Fall()
    {
        bool fell = false;
        for (int i = 0; i < 5; i++)
            fell |= Fall(i);
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
        bool[] shouldGroupDisappear = new bool[50];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GridElement bottle = GetElementAt(x, y);
                // Pour chaque bouteille de la grille
                if (IsBottle(bottle))
                {
                    bool shouldBottleDisappear = false;
                    // Son identifiant est créé de la même manière que celui des danghosts
                    int bottleID = x * 10 + y;

                    // Pour chaque élément autour de la bouteille
                    GetNeighbors(x, y, out int[] neighborXs, out int[] neighborYs);
                    for (int i = 0; i < neighborXs.Length; i++)
                    {
                        // Si c'est un danghost qui doit connecter avec la bouteille
                        if (GetElementAt(neighborXs[i], neighborYs[i]) == DanghostEquivalent(bottle))
                        {
                            // On "groupe" la bouteille avec les danghosts
                            // Ce qui va avoir pour effet de fusionner les groupes liés à la bouteille entre eux
                            // Ce qui permet d'éviter de les compter comme des groupes différents dans les calculs de score
                            int neighborID = groupIDs[neighborXs[i], neighborYs[i]];
                            if (neighborID < bottleID)
                            {
                                if (bottleID < x * 10 + y)
                                    ReplaceInGroupIDs(groupIDs, neighborID, bottleID);
                                else
                                    bottleID = neighborID;
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
                        SetElementAt(x, y, GridElement.EMPTY);

                        // Mais aussi les ghosts autour de la bouteille
                        for (int i = 0; i < neighborXs.Length; i++)
                        {
                            if (GetElementAt(neighborXs[i], neighborYs[i]) == GridElement.GHOST)
                            {
                                SetElementAt(neighborXs[i], neighborYs[i], GridElement.EMPTY);
                            }
                        }
                    }
                    // Le groupe devra disparaître à la fin.
                    // On le fait plus tard et pas dans cette loop au cas où plusieurs bouteilles y sont connectées
                    shouldGroupDisappear[bottleID] = shouldBottleDisappear;
                }
            }
        }

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // Pour chaque ghost/echo restant
                if (GetElementAt(x, y) == GridElement.GHOST)
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
                        SetElementAt(x, y, GridElement.EMPTY);
                    }
                }
            }
        }


        int totalAmount = 0;
        int[] amountByGroup = new int[50];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // Pour chaque danghost qui doit disparaître
                if (groupIDs[x, y] != -1 && shouldGroupDisappear[groupIDs[x, y]])
                {
                    totalAmount++;
                    // On compte le danghost comme faisant partie de ce groupe
                    amountByGroup[groupIDs[x, y]]++;
                    // Et on l'efface.
                    SetElementAt(x, y, GridElement.EMPTY);
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
                    currentMultiplier += 5 + currentChain * 2;
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
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (groupIDs[x, y] != -1)
                {
                    if (GetAbove(x, y, out int xAbove, out int yAbove))
                        TryToGroup(groupIDs, x, y, xAbove, yAbove);
                    if (GetUpperRight(x, y, out int xUR, out int yUR))
                        TryToGroup(groupIDs, x, y, xUR, yUR);
                    if (GetLowerRight(x, y, out int xLR, out int yLR))
                        TryToGroup(groupIDs, x, y, xLR, yLR);
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
        int[,] groupIDs = new int[5, 10];

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (IsDanghost(GetElementAt(x, y)))
                    groupIDs[x, y] = x * 10 + y;
                else
                    groupIDs[x, y] = -1;
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
        if (GetElementAt(x, y) == GetElementAt(x2, y2))
        {
            if (groupIDs[x2, y2] < groupIDs[x, y])
            {
                if (groupIDs[x, y] == x * 10 + y)
                    groupIDs[x, y] = groupIDs[x2, y2];
                else
                    ReplaceInGroupIDs(groupIDs, groupIDs[x, y], groupIDs[x2, y2]);
            }
            else
            {
                if (groupIDs[x2, y2] == x2 * 10 + y2)
                    groupIDs[x2, y2] = groupIDs[x, y];
                else
                    ReplaceInGroupIDs(groupIDs, groupIDs[x2, y2], groupIDs[x, y]);
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
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 10; y++)
                if (groupIDs[x, y] == from) groupIDs[x, y] = to;
    }

    public bool DoOnePopStep()
    {
        if (Fall()) return true;
        return Pop();
    }

    public bool DoAllPopSteps()
    {
        int times = 0;
        while (DoOnePopStep() && times++ < 1000) ;
        if (times >= 1000) Debug.Log("boucle infinie");
        return times != 0;
    }

}

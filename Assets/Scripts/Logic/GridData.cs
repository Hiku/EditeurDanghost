using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public enum GridElement
    {
        EMPTY,
        DANGHOST_BLUE,
        DANGHOST_RED,
        DANGHOST_CYAN,
        DANGHOST_YELLOW,
        DANGHOST_PURPLE,
        DANGHOST_GREEN,
        BOTTLE_BLUE,
        BOTTLE_RED,
        BOTTLE_CYAN,
        BOTTLE_YELLOW,
        BOTTLE_PURPLE,
        BOTTLE_GREEN,
        GHOST
    }
    private List<GridElement[]> grid;
    private GridElement[] nexts;
    private int floorHeight;

    public GridData()
    {
        grid = new List<GridElement[]>();
        nexts = new GridElement[12];
    }

    public GridElement GetNext(int index)
    {
        return nexts[index];
    }

    public void SetNext(int index, GridElement value)
    {
        nexts[index] = value;
    }

    public GridElement GetElementAt(int x, int y)
    {
        int actualY = y + floorHeight;
        if (actualY < 0 || actualY >= grid.Count || x < 0 || x >= 5) return GridElement.EMPTY;
        return grid[actualY][x];
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

    public GridElement[] MakeLine()
    {
        return new GridElement[5];
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
    public GridData Clone()
    {
        GridData clone = new GridData();
        clone.floorHeight = floorHeight;
        clone.grid = GridClone();
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
            if (GetColumnHoles(i) >= 0) return true;
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

    public void GetNeighbors(int x, int y, out int[] neighborXs, out int[] neighborYs)
    {
        List<int> nXs = new List<int>();
        List<int> nYs = new List<int>();
        if (y < 9)
        {
            nXs.Add(x);
            nYs.Add(y + 1);
        }
        if (y > 0)
        {
            nXs.Add(x);
            nYs.Add(y - 1);
        }
        neighborXs = nXs.ToArray();
        neighborYs = nYs.ToArray();
    }

    /*public bool Pop()
    {

    }*/

    public bool DoOnePopStep()
    {
        return Fall();
    }
    public void DoAllPopSteps()
    {
        int times = 0;
        while (DoOnePopStep() && times++ < 1000) ;
        if (times >= 1000) Debug.Log("boucle infinie");
    }

}


using System.Collections.Generic;

public class GridUtils
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
    public static GridElement[] MakeLine()
    {
        return new GridElement[5];
    }
    public static void GetNeighbors(int x, int y, out int[] neighborXs, out int[] neighborYs)
    {
        List<int> nXs = new List<int>();
        List<int> nYs = new List<int>();
        if (GetBelow(x, y, out int xB, out int yB))
        {
            nXs.Add(xB);
            nYs.Add(yB);
        }
        if (GetAbove(x, y, out int xA, out int yA))
        {
            nXs.Add(xA);
            nYs.Add(yA);
        }
        if (GetUpperLeft(x, y, out int xUL, out int yUL))
        {
            nXs.Add(xUL);
            nYs.Add(yUL);
        }
        if (GetUpperRight(x, y, out int xUR, out int yUR))
        {
            nXs.Add(xUR);
            nYs.Add(yUR);
        }
        if (GetLowerLeft(x, y, out int xLL, out int yLL))
        {
            nXs.Add(xLL);
            nYs.Add(yLL);
        }
        if (GetLowerRight(x, y, out int xLR, out int yLR))
        {
            nXs.Add(xLR);
            nYs.Add(yLR);
        }
        neighborXs = nXs.ToArray();
        neighborYs = nYs.ToArray();
    }

    public static bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < 5 && y >= 0 && y < 10;
    }

    public static bool GetBelow(int x, int y, out int x2, out int y2)
    {
        x2 = x;
        y2 = y - 1;
        return IsInBounds(x2, y2);
    }
    public static bool GetAbove(int x, int y, out int x2, out int y2)
    {
        x2 = x;
        y2 = y + 1;
        return IsInBounds(x2, y2);
    }
    public static bool GetUpperLeft(int x, int y, out int x2, out int y2)
    {
        x2 = x - 1;
        y2 = y + x % 2;
        return IsInBounds(x2, y2);
    }
    public static bool GetUpperRight(int x, int y, out int x2, out int y2)
    {
        x2 = x + 1;
        y2 = y + x % 2;
        return IsInBounds(x2, y2);
    }
    public static bool GetLowerLeft(int x, int y, out int x2, out int y2)
    {
        x2 = x - 1;
        y2 = y - (x + 1) % 2;
        return IsInBounds(x2, y2);
    }
    public static bool GetLowerRight(int x, int y, out int x2, out int y2)
    {
        x2 = x + 1;
        y2 = y - (x + 1) % 2;
        return IsInBounds(x2, y2);
    }

    public static bool IsBottle(GridElement element) => element switch
    {
        GridElement.BOTTLE_BLUE => true,
        GridElement.BOTTLE_YELLOW => true,
        GridElement.BOTTLE_CYAN => true,
        GridElement.BOTTLE_RED => true,
        GridElement.BOTTLE_PURPLE => true,
        GridElement.BOTTLE_GREEN => true,
        _ => false
    };

    public static bool IsDanghost(GridElement element) => element switch
    {
        GridElement.DANGHOST_BLUE => true,
        GridElement.DANGHOST_YELLOW => true,
        GridElement.DANGHOST_CYAN => true,
        GridElement.DANGHOST_RED => true,
        GridElement.DANGHOST_PURPLE => true,
        GridElement.DANGHOST_GREEN => true,
        _ => false
    };

    public static GridElement BottleEquivalent(GridElement element) => element switch
    {
        GridElement.DANGHOST_BLUE => GridElement.BOTTLE_BLUE,
        GridElement.DANGHOST_YELLOW => GridElement.BOTTLE_YELLOW,
        GridElement.DANGHOST_CYAN => GridElement.BOTTLE_CYAN,
        GridElement.DANGHOST_RED => GridElement.BOTTLE_RED,
        GridElement.DANGHOST_PURPLE => GridElement.BOTTLE_PURPLE,
        GridElement.DANGHOST_GREEN => GridElement.BOTTLE_GREEN,
        _ => element
    };

    public static GridElement DanghostEquivalent(GridElement element) => element switch
    {
        GridElement.BOTTLE_BLUE => GridElement.DANGHOST_BLUE,
        GridElement.BOTTLE_YELLOW => GridElement.DANGHOST_YELLOW,
        GridElement.BOTTLE_CYAN => GridElement.DANGHOST_CYAN,
        GridElement.BOTTLE_RED => GridElement.DANGHOST_RED,
        GridElement.BOTTLE_PURPLE => GridElement.DANGHOST_PURPLE,
        GridElement.BOTTLE_GREEN => GridElement.DANGHOST_GREEN,
        _ => element
    };

}

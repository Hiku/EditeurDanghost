/*public enum GridElement
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
    GHOST,
    TATANA_CUT,
    KUKUPIN_WALL
}*/

public enum GridElementColor
{
    BLUE,
    RED,
    CYAN,
    YELLOW,
    PURPLE,
    GREEN
}
public enum ElementType
{
    DANGHOST,
    BOTTLE,
    EMPTY,
    GHOST,
    TATANA_CUT,
    KUKUPIN_WALL
}

public class GeneratorGridElement
{
    public static GeneratorGridElement EMPTY = new GeneratorGridElement(ElementType.EMPTY);

    public static GeneratorGridElement GHOST = new GeneratorGridElement(ElementType.GHOST);
    public static GeneratorGridElement TATANA_CUT = new GeneratorGridElement(ElementType.TATANA_CUT);
    public static GeneratorGridElement KUKUPIN_WALL = new GeneratorGridElement(ElementType.KUKUPIN_WALL);

    public static GeneratorGridElement Danghost(GridElementColor color)
    {
        return new GeneratorGridElement(ElementType.DANGHOST, color);
    }

    public static GeneratorGridElement Bottle(GridElementColor color)
    {
        return new GeneratorGridElement(ElementType.BOTTLE, color);
    }

    private ElementType type;
    private GridElementColor? color;
    private GridElementColor? color2;
    private int armor;


    public GeneratorGridElement(ElementType type, GridElementColor? color = null, GridElementColor? color2 = null, int armor = 0)
    {
        this.type = type;
        this.color = color;
        this.color2 = color2;
        this.armor = armor;
    }
    //Fonction égalité

    public ElementType GetElementType()
    {
        return type;
    }
    public GridElementColor? GetColor()
    {
        return color;
    }
    public GridElementColor? GetColor2()
    {
        return color2;
    }


    public GeneratorGridElement GetDanghostEquivalent()
    {
        if (type == ElementType.BOTTLE)
        {
            return new GeneratorGridElement(ElementType.DANGHOST, color, color2, armor);
        }
        else
        {
            return this;
        }
    }
    public GeneratorGridElement GetBottleEquivalent()
    {
        if (type == ElementType.DANGHOST)
        {
            return new GeneratorGridElement(ElementType.BOTTLE, color, color2, armor);
        }
        else
        {
            return this;
        }
    }

    public bool IsEmpty()
    {
        return type == ElementType.EMPTY;
    }

    public bool IsGhost()
    {
        return type == ElementType.GHOST;
    }
    public bool IsDanghost()
    {
        return type == ElementType.DANGHOST;

    }

    public bool IsBottle()
    {
        return type == ElementType.BOTTLE;
    }

    public bool IsSameColorAs(GeneratorGridElement other)
    {
        return (color != null && other.IsColor((GridElementColor)color)) || (color2 != null && other.IsColor((GridElementColor)color2));

    }

    public bool IsColor(GridElementColor color)
    {
        return color == this.color || color == this.color2;
    }

    public int GetSpriteID()
    {
        switch (type)
        {
            case ElementType.DANGHOST:
                return 1 + (int)color;
            case ElementType.BOTTLE:
                return 7 + (int)color;
            case ElementType.EMPTY:
                return 0;
            case ElementType.GHOST:
                return 13;
            case ElementType.TATANA_CUT:
                return 14;
            case ElementType.KUKUPIN_WALL:
                return 15;
        }
        return 0;
    }

    public static bool operator ==(GeneratorGridElement obj1, GeneratorGridElement obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }

        if (ReferenceEquals(obj1, null))
        {
            return false;
        }

        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }
    public static bool operator !=(GeneratorGridElement obj1, GeneratorGridElement obj2) => !(obj1 == obj2);
    public bool Equals(GeneratorGridElement other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return type == other.type && color == other.color && color2 == other.color2 && armor == other.armor;
    }
    public override bool Equals(object obj) => Equals(obj as GeneratorGridElement);


    public override string ToString()
    {
        switch (type)
        {
            case ElementType.DANGHOST:
                return "Y" + ((int)color + 1) + (color2 != null ? ("" + ((int)color2 + 1)) : "");
            case ElementType.BOTTLE:
                return "E" + ((int)color + 1) + (color2 != null ? ("" + ((int)color2 + 1)) : "");
            case ElementType.EMPTY:
                return "";
            case ElementType.GHOST:
                return "P";
            case ElementType.TATANA_CUT:
                return "C";
            case ElementType.KUKUPIN_WALL:
                return "W";
        }
        return "";
    }

    public static GeneratorGridElement FromString(string element)
    {
        if (element.Length == 0)
        {
            return EMPTY;
        }

        switch (element[0])
        {
            case 'Y':
                return Danghost((GridElementColor)(int.Parse("" + element[1]) - 1));
            case 'E':
                return Bottle((GridElementColor)(int.Parse("" + element[1]) - 1));
            case 'P':
                return GHOST;
            case 'C':
                return TATANA_CUT;
            case 'W':
                return KUKUPIN_WALL;
        }
        return EMPTY;
    }

    public override int GetHashCode()
    {
        int hashCode = 479847519;
        hashCode = hashCode * -1521134295 + type.GetHashCode();
        hashCode = hashCode * -1521134295 + color.GetHashCode();
        hashCode = hashCode * -1521134295 + color2.GetHashCode();
        hashCode = hashCode * -1521134295 + armor.GetHashCode();
        return hashCode;
    }
}

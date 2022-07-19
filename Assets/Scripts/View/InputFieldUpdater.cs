using System.Text;
using TMPro;
using UnityEngine;
using static GeneratorGridUtils;

public class InputFieldUpdater : MonoBehaviour
{

    public void UpdateFromGridData(GeneratorGridData data)
    {

        GetComponent<TMP_InputField>().text = GetPrefillGridText(data);
    }

    public void UpdateFromGrowData(GeneratorGridData data)
    {
        GetComponent<TMP_InputField>().text = GetGrowGridText(data);
    }

    public void UpdateFromNextData(GeneratorGridData data)
    {
        GetComponent<TMP_InputField>().text = GetNextText(data);
    }


    public static string GetPrefillGridText(GeneratorGridData data)
    {
        StringBuilder gridTextBuilder = new StringBuilder();
        bool empty = true;

        for (int y = data.GetHighestLine() - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (data.GetElementAt(x, y) != GridElement.EMPTY) empty = false;
            }
            if (empty) continue;

            for (int x = 0; x < width; x++)
            {
                gridTextBuilder.Append(GetTranslation(data.GetElementAt(x, y)));
                if (x != width-1) gridTextBuilder.Append(",");
            }
            if(y!=0)gridTextBuilder.Append("\n");
        }
        return gridTextBuilder.ToString().Replace("\n\n", "\n");
    }
    public static string GetGrowGridText(GeneratorGridData data)
    {
        StringBuilder gridTextBuilder = new StringBuilder();
        for (int y = -1; y >= data.GetLowestLine(); y--)
        {
            for (int x = 0; x < width; x++)
            {
                gridTextBuilder.Append(GetTranslation(data.GetElementAt(x, y)));
                if (x != width-1) gridTextBuilder.Append(",");
            }
            if (y != data.GetLowestLine()) gridTextBuilder.Append("\n");
        }
        string gridText = gridTextBuilder.ToString();
        string toRemove = "\n";
        for(int i = 0; i< width-1; i++) toRemove += ",";
        while(gridText.EndsWith(toRemove))gridText = gridText.Substring(0,gridText.Length- width);
        return gridText;
    }
    public static string GetNextText(GeneratorGridData data)
    {
        StringBuilder gridTextBuilder = new StringBuilder();
        int nextAmount = 0;
        for (int index = 0; index < 12; index++)
        {
            if (data.GetNext(index) != GridElement.EMPTY) nextAmount = index + 1;
        }

        for (int index = 0; index < nextAmount; index++)
        {
            gridTextBuilder.Append(GetTranslation(data.GetNext(index)) + "\n");
        }
        return gridTextBuilder.ToString();
    }

    public void FillWithPrefillGridText(GeneratorGridData data)
    {
        string text = GetComponent<TMP_InputField>().text;
        text = text.Replace("\r", "");

        string[] lines = text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        data.ClearAbove(lines.Length - 1);
        for (int y = 0; y < lines.Length; y++)
        {
            string[] elements = lines[y].Split(',');
            for (int x = 0; x < elements.Length; x++)
            {
                data.SetElementAt(x, lines.Length - 1 - y, GetTranslation(elements[x]));

            }
        }
    }
    public void FillWithGrowGridText(GeneratorGridData data)
    {
        string text = GetComponent<TMP_InputField>().text;
        text = text.Replace("\r", "");

        string[] lines = text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        data.ClearBelow(-lines.Length);

        for (int y = 0; y < lines.Length; y++)
        {
            string[] elements = lines[y].Split(',');
            for (int x = 0; x < elements.Length; x++)
            {
                data.SetElementAt(x, -y - 1, GetTranslation(elements[x]));
            }
        }
    }
    public void FillWithNextText(GeneratorGridData data)
    {
        string text = GetComponent<TMP_InputField>().text;
        text = text.Replace("\r", "");
        string[] elements = text.Split(new char[] { '\n' }, 12);
        for (int index = 0; index < 12; index++)
        {
            if(index >= elements.Length) 
                data.SetNext(index, GridElement.EMPTY);
            else
                data.SetNext(index, GetTranslation(elements[index]));
        }
    }




    public static string GetTranslation(GridElement element) => element switch
    {
        GridElement.EMPTY => "",
        GridElement.DANGHOST_BLUE => "Y3",
        GridElement.DANGHOST_RED => "Y4",
        GridElement.DANGHOST_CYAN => "Y2",
        GridElement.DANGHOST_YELLOW => "Y5",
        GridElement.DANGHOST_PURPLE => "Y0",
        GridElement.DANGHOST_GREEN => "Y1",
        GridElement.BOTTLE_BLUE => "E3",
        GridElement.BOTTLE_RED => "E4",
        GridElement.BOTTLE_CYAN => "E2",
        GridElement.BOTTLE_YELLOW => "E5",
        GridElement.BOTTLE_PURPLE => "E0",
        GridElement.BOTTLE_GREEN => "E1",
        GridElement.GHOST => "P",
        _ => ""
    };
    public GridElement GetTranslation(string element) => element switch
    {
        "" => GridElement.EMPTY,
        "Y3" => GridElement.DANGHOST_BLUE,
        "Y4" => GridElement.DANGHOST_RED,
        "Y2" => GridElement.DANGHOST_CYAN,
        "Y5" => GridElement.DANGHOST_YELLOW,
        "Y0" => GridElement.DANGHOST_PURPLE,
        "Y1" => GridElement.DANGHOST_GREEN,
        "E3" => GridElement.BOTTLE_BLUE,
        "E4" => GridElement.BOTTLE_RED,
        "E2" => GridElement.BOTTLE_CYAN,
        "E5" => GridElement.BOTTLE_YELLOW,
        "E0" => GridElement.BOTTLE_PURPLE,
        "E1" => GridElement.BOTTLE_GREEN,
        "P" => GridElement.GHOST,
        _ => GridElement.EMPTY
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

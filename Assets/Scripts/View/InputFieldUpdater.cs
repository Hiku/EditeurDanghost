using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GridUtils;

public class InputFieldUpdater : MonoBehaviour
{

    public void UpdateFromGridData(GridData data)
    {

        GetComponent<TMP_InputField>().text = GetPrefillGridText(data);
    }

    public string GetPrefillGridText(GridData data)
    {
        StringBuilder gridTextBuilder = new StringBuilder();

        for (int y = 9; y >= 0; y--)
        {
            for (int x = 0; x < 5; x++)
            {
                gridTextBuilder.Append(GetTranslation(data.GetElementAt(x, y)));
                if (x != 4) gridTextBuilder.Append(",");
            }
            gridTextBuilder.Append("\n");
        }
        return gridTextBuilder.ToString();
    }

    /*public void FillWithGridText(GridData data)
    {
        string text = GetComponent<TMP_InputField>().text;
        int line = 0;

        while (text.Length > 0)
        {
            switch (text[0])
            {
                case '\n':
                    break;
                case ',':
                    break;

            }
        }
        for (int y = 9; y >= 0; y--)
        {

            for (int x = 0; x < 5; x++)
            {
                gridTextBuilder.Append(GetTranslation(data.GetElementAt(x, y)));
                if (x != 4) gridTextBuilder.Append(",");
            }
            gridTextBuilder.Append("\n");
        }
        return gridTextBuilder.ToString();

    }*/

    public string GetTranslation(GridElement element) => element switch
    {
        GridElement.EMPTY => "",
        GridElement.DANGHOST_BLUE => "Y0",
        GridElement.DANGHOST_RED => "Y1",
        GridElement.DANGHOST_CYAN => "Y2",
        GridElement.DANGHOST_YELLOW => "Y3",
        GridElement.DANGHOST_PURPLE => "Y4",
        GridElement.DANGHOST_GREEN => "Y5",
        GridElement.BOTTLE_BLUE => "E0",
        GridElement.BOTTLE_RED => "E1",
        GridElement.BOTTLE_CYAN => "E2",
        GridElement.BOTTLE_YELLOW => "E3",
        GridElement.BOTTLE_PURPLE => "E4",
        GridElement.BOTTLE_GREEN => "E5",
        GridElement.GHOST => "P",
        _ => ""
    };
    public GridElement GetTranslation(string element) => element switch
    {
        "" => GridElement.EMPTY,
        "Y0" => GridElement.DANGHOST_BLUE,
        "Y1" => GridElement.DANGHOST_RED,
        "Y2" => GridElement.DANGHOST_CYAN,
        "Y3" => GridElement.DANGHOST_YELLOW,
        "Y4" => GridElement.DANGHOST_PURPLE,
        "Y5" => GridElement.DANGHOST_GREEN,
        "E0" => GridElement.BOTTLE_BLUE,
        "E1" => GridElement.BOTTLE_RED,
        "E2" => GridElement.BOTTLE_CYAN,
        "E3" => GridElement.BOTTLE_YELLOW,
        "E4" => GridElement.BOTTLE_PURPLE,
        "E5" => GridElement.BOTTLE_GREEN,
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

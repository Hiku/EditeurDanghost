using UnityEngine;
using UnityEngine.UI;
using static GeneratorGridUtils;

public class PaintSelector : MonoBehaviour
{

    [SerializeField]
    Button[] buttons;
    GeneratorGridElement[] buttonElements;

    GeneratorGridElement currentPaint;

    // Start is called before the first frame update
    void Start()
    {
        buttonElements = new GeneratorGridElement[buttons.Length];
        for (int i = 0; i < 6; i++)
        {
            buttonElements[i] = GeneratorGridElement.Danghost((GridElementColor)i);
        }
        for (int i = 6; i < 12; i++)
        {
            buttonElements[i] = GeneratorGridElement.Bottle((GridElementColor)(i-6));
        }
        buttonElements[12] = GeneratorGridElement.GHOST;
        for (int i = 0; i < buttons.Length; i++)
        {
            int b = i;
            buttons[i].onClick.AddListener(() => 
            {
                SelectPaint(buttonElements[b]);
            });
        }
        SelectPaint(buttonElements[0]);
    }

    public void SelectPaint(GeneratorGridElement element)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonElements[i] == element)
            {
                buttons[i].GetComponent<Image>().color = new Color(35f / 255f, 31f / 255f, 29f / 255f);
            }
            else
            {
                buttons[i].GetComponent<Image>().color = Color.white;
            }
        }
        currentPaint = element;
    }

    public GeneratorGridElement GetCurrentPaint()
    {
        return currentPaint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.BLUE));
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.RED));
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.CYAN));
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.YELLOW));
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.PURPLE));
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectPaint(GeneratorGridElement.Danghost(GridElementColor.GREEN));
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            SelectPaint(GetCurrentPaint().GetBottleEquivalent());
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            SelectPaint(GetCurrentPaint().GetDanghostEquivalent());
    }
}

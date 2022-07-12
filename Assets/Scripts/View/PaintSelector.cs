using UnityEngine;
using UnityEngine.UI;
using static GridUtils;

public class PaintSelector : MonoBehaviour
{

    [SerializeField]
    Button[] buttons;
    [SerializeField]
    GridElement[] buttonElements;

    GridElement currentPaint;

    // Start is called before the first frame update
    void Start()
    {
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

    public void SelectPaint(GridElement element)
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

    public GridElement GetCurrentPaint()
    {
        return currentPaint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SelectPaint(GridElement.DANGHOST_BLUE);
        if (Input.GetKeyDown(KeyCode.Z)) SelectPaint(GridElement.DANGHOST_RED);
        if (Input.GetKeyDown(KeyCode.E)) SelectPaint(GridElement.DANGHOST_CYAN);
        if (Input.GetKeyDown(KeyCode.R)) SelectPaint(GridElement.DANGHOST_YELLOW);
        if (Input.GetKeyDown(KeyCode.T)) SelectPaint(GridElement.DANGHOST_PURPLE);
        if (Input.GetKeyDown(KeyCode.Y)) SelectPaint(GridElement.DANGHOST_GREEN);
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            SelectPaint(BottleEquivalent(GetCurrentPaint()));
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            SelectPaint(DanghostEquivalent(GetCurrentPaint()));
    }
}

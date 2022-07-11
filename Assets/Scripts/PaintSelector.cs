using UnityEngine;
using UnityEngine.UI;
using static GridData;

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

    }
}

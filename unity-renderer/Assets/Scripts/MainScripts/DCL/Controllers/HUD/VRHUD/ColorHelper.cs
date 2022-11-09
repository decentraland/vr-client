using UnityEngine;
using UnityEngine.UI;

public class ColorHelper : MonoBehaviour
{
    [SerializeField]
    private Color selectedColor;
    private Color defaultColor;
    private Image image;
    private bool selected;
    private void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void OnToggle()
    {
        selected = !selected;
        image.color = selected ? selectedColor : defaultColor;
    }
}

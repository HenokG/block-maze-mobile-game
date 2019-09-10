using UnityEngine;

public class Theme : MonoBehaviour
{
    public Color BgColor;
    public Color GridColor;
    public Color MainColor;

    public Theme(Color bgColor, Color gridColor, Color mainColor)
    {
        BgColor = bgColor;
        GridColor = gridColor;
        MainColor = mainColor;
    }
}
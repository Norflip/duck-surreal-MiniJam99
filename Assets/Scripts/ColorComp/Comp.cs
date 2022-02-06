using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp : MonoBehaviour
{
    public PaintingImage image;

    public Texture2D result;

    void Start ()
    {
        Round(image);
    }

    void Round (PaintingImage image)
    {
        result = new Texture2D(image.image.width, image.image.height);
        result.filterMode = FilterMode.Point;

        for (int y = 0; y < image.image.height; y++)
        {
            for (int x = 0; x < image.image.width; x++)
            {
                Color i = GetClosestColor(image.image.GetPixel(x,y), image.palette);
                result.SetPixel(x,y, i);
            }
        }

        result.Apply();
    }

/*
    int GetClosestColor (Color c, Color[] palette)
    {
        Color.RGBToHSV(c, out float H, out float S, out float V);

        var num1 = ColorNum(target);
        var diffs = palette.Select(n => Mathf.Abs(ColorNum(n) - num1) + 
                                    getHueDistance(n.GetHue(), H) );

        var diffMin = diffs.Min(x => x);
        return diffs.ToList().FindIndex(n => n == diffMin);
    }

        // color brightness as perceived:
    float getBrightness(Color c)  
        { return (c.r * 0.299f + c.g * 0.587f + c.b *0.114f) / 256f;}

    // distance between two hues:
    float getHueDistance(float hue1, float hue2)
    { 
        float d = Mathf.Abs(hue1 - hue2); return d > 180 ? 360 - d : d; }

    //  weighed only by saturation and brightness (from my trackbars)
    float ColorNum(Color c) { 
        return c.GetSaturation() * factorSat + getBrightness(c) * factorBri; 
        
        }

    // distance in RGB space
    int ColorDiff(Color c1, Color c2) 
        { return  (int ) Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r) 
                                + (c1.g - c2.g) * (c1.g - c2.g)
                                + (c1.b - c2.b) * (c1.b - c2.b)); }

*/

    private static Color GetClosestColor(Color baseColor, Color[] colorArray)
    {
        var colors = colorArray.Select(x => new {Value = x, Diff = GetDiff(x, baseColor)}).ToList();
        var min = colors.Min(x => x.Diff);
        
        return colors.Find(x => x.Diff == min).Value;
    }

    private static int GetDiff(Color color, Color baseColor)
    {
        int a = Mathf.RoundToInt(255 * color.a) - Mathf.RoundToInt(2555 * baseColor.a),
            r = Mathf.RoundToInt(255 * color.r) - Mathf.RoundToInt(2555 * baseColor.r),
            g = Mathf.RoundToInt(255 * color.g) - Mathf.RoundToInt(2555 * baseColor.g),
            b = Mathf.RoundToInt(255 * color.b) - Mathf.RoundToInt(2555 * baseColor.b);
        return a*a + r*r + g*g + b*b;
    }

}

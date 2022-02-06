using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp : MonoBehaviour
{
    public ComputeShader round_cs;
    public ComputeShader compare_cs;
    
    public PaintingImage image;
    public Texture2D result0;
    public Texture2D result1;

    void Start () {
        result0 = Average(image.image);
    }

    public RenderTexture RoundedTexture (PaintingImage image)
    {
        RenderTexture rt = new RenderTexture(image.image.width, image.image.height, 0);
        
        round_cs.SetTexture(0, "_Source", image.image);
        round_cs.SetTexture(0, "_Target", rt);
        round_cs.SetInt("_Radius", 5);

        int thrx = Mathf.CeilToInt(image.image.width / 32.0f);
        int thry = Mathf.CeilToInt(image.image.height / 32.0f);
        round_cs.Dispatch(0, thrx, thry, 1);

        return rt;
    }

    Texture2D Round (Texture2D img) {
        Texture2D  result0 = new Texture2D(img.width, img.height);
        result0.filterMode = FilterMode.Point;

        for (int y = 0; y < img.height; y++)
        {
            for (int x = 0; x < img.width; x++)
            {
                Color i = GetClosestColor(img.GetPixel(x,y), image.palette);
                result0.SetPixel(x,y, i);
            }
        }

        result0.Apply();
        return result0;
    }

    Texture2D Average (Texture2D texture) {
        Texture2D result1 = new Texture2D(image.image.width, image.image.height);
        result1.filterMode = FilterMode.Point;
        
        int radius = 6;
        int sumdiv = (radius * 2 + 1) * (radius * 2 + 1);

        for (int y = 0; y < image.image.height; y++) {
            for (int x = 0; x < image.image.width; x++) {
                Vector4 sum = Vector4.zero;
                
                for (int dx = -radius; dx <= radius; dx++)                {
                    for (int dy = -radius; dy <= radius; dy++) {
                        sum += (Vector4)texture.GetPixel(x + dx,y + dy);
                    }
                }

                sum /= (float)sumdiv;
                Color i = GetClosestColor((Color)sum, image.palette);
                result1.SetPixel(x,y, i);
            }
        }

        result1.Apply();
        return result1;
    }

    private static Color GetClosestColor(Color baseColor, Color[] colorArray)
    {
        var colors = colorArray.Select(x => new {Value = x, Diff = GetDiff(x, baseColor)}).ToList();
        var min = colors.Min(x => x.Diff);
        
        return colors.Find(x => x.Diff == min).Value;
    }

    private static int GetDiff(Color color, Color baseColor)
    {
        int a = Mathf.RoundToInt(255 * color.a) - Mathf.RoundToInt(255 * baseColor.a),
            r = Mathf.RoundToInt(255 * color.r) - Mathf.RoundToInt(255 * baseColor.r),
            g = Mathf.RoundToInt(255 * color.g) - Mathf.RoundToInt(255 * baseColor.g),
            b = Mathf.RoundToInt(255 * color.b) - Mathf.RoundToInt(255 * baseColor.b);
        return a*a + r*r + g*g + b*b;
    }

}

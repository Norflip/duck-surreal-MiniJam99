using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp : MonoBehaviour
{
    public ComputeShader round_cs;
    public ComputeShader compare_cs;

    public float Compare (RenderTexture rt, PaintingImage image)
    {
        int maxValue = image.image.width * image.image.height * 10;
        int[] c= new int[1]{0};
        ComputeBuffer counter = new ComputeBuffer(1, sizeof(int));
        counter.SetData(c);

        compare_cs.SetTexture(0, "_TextureA", rt);
        compare_cs.SetTexture(0, "_TextureB", image.image);
        compare_cs.SetBuffer(0, "_Counter", counter);

        int thrx = Mathf.CeilToInt(image.image.width / 8.0f);
        int thry = Mathf.CeilToInt(image.image.height / 8.0f);
        compare_cs.Dispatch(0, thrx, thry, 1);

        counter.GetData(c);
        float percentage = c[0] / maxValue;
        counter.Dispose();
        return percentage;
    }
    
    public RenderTexture RoundTextureToPalette (PaintingImage image)
    {
        RenderTexture rt = new RenderTexture(image.image.width, image.image.height, 0);
        ComputeBuffer paletteBuffer = new ComputeBuffer(image.palette.Length, sizeof(float) * 4);
        paletteBuffer.SetData(image.palette);

        round_cs.SetTexture(0, "_Source", image.image);
        round_cs.SetTexture(0, "_Target", rt);
        round_cs.SetBuffer(0, "_Palette", paletteBuffer);

        int thrx = Mathf.CeilToInt(image.image.width / 8.0f);
        int thry = Mathf.CeilToInt(image.image.height / 8.0f);
        round_cs.Dispatch(0, thrx, thry, 1);

        paletteBuffer.Dispose();
        return rt;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp : MonoBehaviour
{
    public PaintingImage image0;
    public PaintingImage image1;

    public ComputeShader round_cs;
    public ComputeShader compare_cs;

    private void Awake() {
        Test(image0, image0);
        Test(image0, image1);
    }

    public void Test (PaintingImage m0, PaintingImage m1)
    {
        //RenderTexture a = RoundTextureToPalette(m0);
        //RenderTexture b = RoundTextureToPalette(m1);

        int maxValue = m0.image.width * m0.image.height * 10;
        int[] c= new int[1]{0};
        ComputeBuffer counter = new ComputeBuffer(1, sizeof(int));
        counter.SetData(c);

        compare_cs.SetTexture(0, "_TextureA", m0.image);
        compare_cs.SetTexture(0, "_TextureB", m1.image);
        compare_cs.SetBuffer(0, "_Counter", counter);

        int thrx = Mathf.CeilToInt(m0.image.width / 8.0f);
        int thry = Mathf.CeilToInt(m0.image.height / 8.0f);
        compare_cs.Dispatch(0, thrx, thry, 1);
        counter.GetData(c);

        float percentage = (float)c[0] / (m0.image.width * m0.image.height * 10.0f); //1.0f - (c[0] / 255.0f) / (m0.image.width * m0.image.height);

        Debug.Log("Count: " + c[0]);
        Debug.Log("D: " + ((m0.image.width * m0.image.height * 10.0f)));
        Debug.Log("PERCENTAGE: " + percentage);

        counter.Dispose();
    }

    public float Compare (RenderTexture rt, PaintingImage image)
    {
       //RenderTexture a = RoundTextureToPalette(m0);
        //RenderTexture b = RoundTextureToPalette(m1);
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

        float percentage = (float)c[0] / (image.image.width * image.image.height * 10.0f); //1.0f - (c[0] / 255.0f) / (m0.image.width * m0.image.height);

        counter.Dispose();
        return percentage;
    }
    
    public RenderTexture RoundTextureToPalette (PaintingImage image)
    {
        RenderTexture rt = new RenderTexture(image.image.width, image.image.height, 0);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.Create();

        ComputeBuffer paletteBuffer = new ComputeBuffer(image.palette.Length, sizeof(float) * 4);
        paletteBuffer.SetData(image.palette);

        round_cs.SetTexture(0, "_Source", image.image);
        round_cs.SetTexture(0, "_Result", rt);
        round_cs.SetBuffer(0, "_Palette", paletteBuffer);

        int thrx = Mathf.CeilToInt(image.image.width / 8.0f);
        int thry = Mathf.CeilToInt(image.image.height / 8.0f);
        round_cs.Dispatch(0, thrx, thry, 1);

        paletteBuffer.Dispose();
        return rt;
    }
}

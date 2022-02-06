using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelect : MonoBehaviour
{
    public Image[] tiles;
    public float selectedScale = 1.2f;

    System.Action<Color> callback;
    PaintingImage image;
    int selected;

    Vector2 defaultSize;

    public void Populate (PaintingImage image, System.Action<Color> selectionCallback)
    {
        this.image = image;
        this.callback = selectionCallback;
        this.selected = 0;
        defaultSize = tiles[0].rectTransform.sizeDelta;
        
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].color = image.palette[i];
        }

        Select(0);
    }

    public void Select(int index)
    {
        tiles[selected].rectTransform.sizeDelta = defaultSize;
        selected = index;
        tiles[selected].rectTransform.sizeDelta = defaultSize * selectedScale;
        callback?.Invoke(image.palette[selected]);
    }

    void Update ()
    {
        int s = selected;
        if(Input.GetKeyDown(KeyCode.E))
            s ++;

        if(Input.GetKeyDown(KeyCode.Q))
            s--;

        for (int i = 0; i < 8; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
                s = i;
        }

        s %= tiles.Length;
        if(s != selected)
        {
            Select(s);
        }
    }
}

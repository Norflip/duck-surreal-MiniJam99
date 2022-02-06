using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Player player;
    public MiniDuckController miniducks;
    public Painting painting;
    public ColorSelect select;

    public PaintingImage[] paintings;
    public int selectedPainting;
    
    public bool startOnStart = true;

    void Start () {
        if(startOnStart)
            StartLevel();
    }

    public void StartLevel ()
    {
        selectedPainting = PlayerPrefs.GetInt("SelectedIndex", selectedPainting);
        Debug.Assert(selectedPainting >= 0 && selectedPainting < paintings.Length);

        for (int i = 0; i < paintings[selectedPainting].palette.Length; i++)
        {
            Color c = paintings[selectedPainting].palette[i];
            c.a = 1.0f;
            paintings[selectedPainting].palette[i] = c;
        }

        select.Populate(paintings[selectedPainting], player.SelectColor);

        int xres = paintings[selectedPainting].image.width;
        int yres = paintings[selectedPainting].image.height;
        Debug.Log($"x: {xres} y {yres} aspect: {(float)xres / (float)yres}");
        painting.Initialize(xres, yres);
        player.run = true;
        miniducks.run = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Player player;
    public MiniDuckController miniducks;
    public Painting painting;

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

        int xres = paintings[selectedPainting].image.width;
        int yres = paintings[selectedPainting].image.height;
        Debug.Log($"x: {xres} y {yres} aspect: {(float)xres / (float)yres}");
        painting.Initialize(xres, yres);
        player.run = true;
        miniducks.run = true;
    }
}

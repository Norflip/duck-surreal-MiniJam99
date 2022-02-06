using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public Material clipboardMaterial;
    public GameObject clipBoardObject;

    public Player player;
    public MiniDuckController miniducks;
    public Painting painting;
    public ColorSelect select;

    public PaintingImage[] paintings;
    public int selectedPainting;
    
    public bool startOnStart = true;

/*
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName ([In, Out] OpenFileName ofn);
*/
    public RawImage refPic, yourPic;
    public Image timerBar;
    public float maxTime = 5f;
    float timeLeft;
    public GameObject[] stuffToDeActivate;
    public GameObject endScreen;

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
        
        painting.Initialize(paintings[selectedPainting]);
        player.run = true;
        miniducks.run = true;


        for (int i = 0; i < stuffToDeActivate.Length; i++)
            stuffToDeActivate[i].SetActive(true);
        //timerBar = GetComponent<Image>();
        timeLeft = maxTime;
        endScreen.SetActive(false);

        Texture newtext = paintings[selectedPainting].image;
        clipBoardObject.GetComponent<MeshRenderer>().material.mainTexture = newtext;        
    }

    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            Debug.Break();
#else
            Application.Quit();
#endif
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //ShowFiledialogToSaveImage();
        }


        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            EndLevel();
        }

    }

    public void EndLevel ()
    {
        for (int i = 0; i < stuffToDeActivate.Length; i++)
            stuffToDeActivate[i].SetActive(false);

        Physics.autoSimulation = false;
       // Time.timeScale = 0;
        endScreen.SetActive(true);
        refPic.texture = paintings[selectedPainting].image;
        yourPic.texture = painting.resultTexture;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
}

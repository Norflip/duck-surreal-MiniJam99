using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{   


    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

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

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName ([In, Out] OpenFileName ofn);

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

    public Texture2D tosave;


    // https://forum.unity.com/threads/openfiledialog-in-runtime.459474/
    void ShowFiledialogToSaveImage ()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = "Open Project";
        ofn.defExt = "png";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST|OFN_NOCHANGEDIR
        
        if(GetSaveFileName(ofn))
        {
            Debug.Log("Selected file with full path: {0}" + ofn.file);

            int w = painting.resultTexture.width;
            int h =  painting.resultTexture.height;
            
            //RenderTexture temp = RenderTexture.GetTemporary(w, h, 0,)

            tosave = new Texture2D(w, h, TextureFormat.ARGB32, false);
            RenderTexture.active = painting.resultTexture;
            tosave.ReadPixels( new Rect(0, 0, w, h), 0, 0);

           // Graphics.Blit(painting.resultTexture, tosave);
            

            byte[] bytes = tosave.EncodeToPNG();
            System.IO.File.WriteAllBytes(ofn.file, bytes);
        }
    }
}

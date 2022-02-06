using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClipboardSelection : MonoBehaviour
{
    public GameObject[] clipBoards;
    public int selectedBoard = 0;

    public void Start()
    {
        clipBoards[0].SetActive(true);
    }
    public void NextBoard()
    {
        clipBoards[selectedBoard].SetActive(false);
        selectedBoard = (selectedBoard + 1) % clipBoards.Length;
        Debug.Log(selectedBoard);
        clipBoards[selectedBoard].SetActive(true);
    }

    public void PreviousBoard()
    {
        clipBoards[selectedBoard].SetActive(false);
        selectedBoard--;
        if(selectedBoard < 0)
        {
            selectedBoard += clipBoards.Length;
        }
        clipBoards[selectedBoard].SetActive(true);
    }

    // Start game ? 
    public void StartGame()
    {
        PlayerPrefs.SetInt("SelectedIndex", selectedBoard);
       // SceneManager.LoadScene(1);

    }
}

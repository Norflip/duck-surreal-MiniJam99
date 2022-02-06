using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Break();
#else
            Application.Quit();
#endif
    }
}

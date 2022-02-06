using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimerScript : MonoBehaviour
{
    public Image timerBar;
    public float maxTime = 5f;
    float timeLeft;
    public GameObject[] stuffToDeActivate;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < stuffToDeActivate.Length; i++)
            stuffToDeActivate[i].SetActive(true);
      timerBar = GetComponent<Image>();
      timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        } else
        {
            for (int i = 0; i < stuffToDeActivate.Length; i++)
                stuffToDeActivate[i].SetActive(false);
            Time.timeScale = 0;
        }
    }
}

using UnityEngine;
using TMPro;
using System;

public class PrintTimerMenu : MonoBehaviour
{
    [SerializeField] int index;
    TextMeshProUGUI txt;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
       // txt.text = "Best time:\n" + TimeSpan.FromSeconds((int)TimerManager.Instance.bestTime[index]).ToString();
        txt.text = "Best time:\n" + FormatTime(TimerManager.Instance.bestTime[index]);
    }
    string FormatTime(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }
}
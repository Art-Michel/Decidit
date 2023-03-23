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
        txt.text = "Time : " + TimeSpan.FromSeconds((int)TimerManager.instance.bestTime[index]).ToString();
    }
}
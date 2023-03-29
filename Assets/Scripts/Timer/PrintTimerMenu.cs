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
        txt.text = "Best time:\n" + TimeSpan.FromSeconds((int)TimerManager.Instance.bestTime[index]).ToString();
    }
}
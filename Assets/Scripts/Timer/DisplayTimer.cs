using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class DisplayTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt;
    PlayerInputMap _inputs;
    float currentTime;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        _inputs = new PlayerInputMap();
        _inputs.MenuNavigation.Score.started += _ => EnableScore();
    }

    // void Start()
    // {
    //     txt = GetComponent<TextMeshProUGUI>();

    //     if (txt.gameObject.name != "DisplayTimerEndGame")
    //         txt.enabled = false;
    // }

    // Update is called once per frame
    void Update()
    {
        /*if (txt)
            txt.text = System.TimeSpan.FromSeconds((int)TimerManager.Instance.time).ToString();*/

        currentTime += Time.deltaTime;
        if (txt)
            txt.text = FormatTime(currentTime);
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

    private void EnableScore()
    {
        if (!TimerManager.Instance.endGame)
            txt.enabled = !txt.enabled;
        else
            txt.enabled = true;
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
}

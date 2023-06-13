using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] float _delay = 0.5f;
    float _delayT = 0.0f;

    bool _isStarting;

    void OnEnable()
    {
        PlayerManager.Instance.CanPause = false;
        _isStarting = true;
        _delayT = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStarting)
            HandleStartup();
    }

    private void HandleStartup()
    {
        _delayT += Time.unscaledDeltaTime;
        float i = Mathf.InverseLerp(0.0f, _delay, _delayT);
        Time.timeScale = Mathf.Lerp(1.0f, 0.0f, i);

        if (_delayT >= _delay)
            Enable();
    }

    protected virtual void Enable()
    {
        _isStarting = false;
        MenuManager.Instance.StartMenuing();
        PlayerManager.Instance.StopGame();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] float _delay = 0.5f;
    float _delayT = 0.0f;

    bool _isStarting;

    public void Enable()
    {
        _isStarting = true;
        _delayT = 0.0f;
    }

    void Update()
    {
        if (_isStarting)
            HandleStartup();
    }

    private void HandleStartup()
    {
        _delayT += Time.unscaledDeltaTime;
        float i = Mathf.InverseLerp(0.0f, _delay, _delayT);

        if (_delayT >= _delay)
            _isStarting = false;
    }

}

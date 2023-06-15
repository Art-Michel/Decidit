using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //Refs
    // private PlayerInputMap _inputs;
    [SerializeField] CanvasGroup _canvas;
    [SerializeField] AnimationCurve _appearanceCurve;

    [SerializeField] float _minimumLifeSpan = 2.0f;
    //Delay
    [SerializeField] float _delay = 0.5f;
    float _t = 0.0f;
    bool _active;

    public void Enable()
    {
        _canvas.gameObject.SetActive(true);
        _active = true;
    }

    public void Disable()
    {
        _active = false;
    }

    void Update()
    {
        if (_active)
            HandleStartup();
        else
            HandleDisappearance();

        _minimumLifeSpan -= Time.deltaTime;
        float i = Mathf.InverseLerp(0.0f, _delay, _t);
        _canvas.alpha = _appearanceCurve.Evaluate(i);
    }

    private void HandleStartup()
    {
        _t += Time.deltaTime;
        _t = Mathf.Clamp(_t, 0, _delay);

    }

    private void HandleDisappearance()
    {
        if (_minimumLifeSpan > 0)
            return;

        _t -= Time.deltaTime;
        _t = Mathf.Clamp(_t, 0, _delay);

        if (_t <= 0.0f)
            gameObject.SetActive(false);
    }

    //     void OnEnable()
    //     {
    //         _inputs.Enable();
    //     }
    //     void OnDisable()
    //     {
    //         _inputs.Disable();
    //     }
}
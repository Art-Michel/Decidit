using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //Refs
    [SerializeField] CanvasGroup _canvas;
    private PlayerInputMap _inputs;
    [SerializeField] AnimationCurve _appearanceCurve;

    //Delay
    [SerializeField] float _delay = 0.5f;
    float _delayT = 0.0f;
    bool _isStarting;

    public void Enable()
    {
        _inputs = new PlayerInputMap();
        _inputs.Actions.Interact.started += _ => Dismiss();

        _canvas.alpha = 0.0f;
        _canvas.gameObject.SetActive(true);
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
        _delayT += Time.deltaTime;
        float i = Mathf.InverseLerp(0.0f, _delay, _delayT);
        _canvas.alpha = _appearanceCurve.Evaluate(i);

        if (_delayT >= _delay)
            _isStarting = false;
    }

    private void Dismiss()
    {
        _canvas.gameObject.SetActive(false);
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    PlayerInputMap _inputs;
    [SerializeField] TextMeshProUGUI _TimescaleDebugUi;
    bool isFocused;
    bool _is30fps;

    void Awake()
    {
        _inputs = new PlayerInputMap();
#if UNITY_EDITOR
        _inputs.Debugging.Unfocus.started += _ => FocusUnfocus();
        _inputs.Debugging.ChangeFramerate.started += _ => ChangeFramerate();
        _inputs.Debugging.ChangeTimeScale.started += _ => ChangeTimeScale();
#endif
    }

    void Start()
    {
        // curseurs
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Framerate
        _is30fps = false;
    }

    void ChangeFramerate()
    {
        if (_is30fps)
        {
            _is30fps = false;
            Application.targetFrameRate = 144;
        }
        else
        {
            _is30fps = true;
            Application.targetFrameRate = 30;
        }
    }

    void ChangeTimeScale()
    {
        var direction = _inputs.Debugging.ChangeTimeScale.ReadValue<float>();
        if (Mathf.Sign(direction) > 0) Time.timeScale = Mathf.Clamp(Time.timeScale + .1f, 0.01f, 10);
        else if (Mathf.Sign(direction) < 0) Time.timeScale = Mathf.Clamp(Time.timeScale - .1f, 0.01f, 10);

        if (_TimescaleDebugUi)
            _TimescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F1"));
    }

    void FocusUnfocus()
    {
        if (isFocused)
        {
            isFocused = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            isFocused = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #region Enable Disable Inputs
    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}

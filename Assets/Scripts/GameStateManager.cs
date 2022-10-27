using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    PlayerInputMap _inputs;
    bool isFocused;
    bool _is30fps;

    private void Awake()
    {
        _inputs = new PlayerInputMap();
#if UNITY_EDITOR
        _inputs.Debugging.Unfocus.started += _ => FocusUnfocus();
        _inputs.Debugging.ChangeFramerate.started += _ => ChangeFramerate();
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

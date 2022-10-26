using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    PlayerInputMap _inputs;
    bool isFocused;
    float _cooldown = 3f;
    bool _is30fps = false;
    [SerializeField] bool _debugFramerate;

    private void Awake()
    {
        _inputs = new PlayerInputMap();
        _inputs.FirstPersonCamera.Unfocus.started += _ => FocusUnfocus();
    }
    void Start()
    {
        #region curseurs
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endregion
    }

    void Update()
    {
        if (_debugFramerate)
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown <= 0)
                ChangeFramerate();
        }
    }

    void ChangeFramerate()
    {
        _cooldown = 3f;
        if (_is30fps)
        {
            _is30fps = false;
            Application.targetFrameRate = 180;
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

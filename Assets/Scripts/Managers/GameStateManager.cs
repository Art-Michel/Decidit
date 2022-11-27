using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    PlayerInputMap _inputs;
    [SerializeField] GameObject _DebuggingCanvas;
    [SerializeField] TextMeshProUGUI _timescaleDebugUi;
    bool _isFocused;
    bool _is30fps;
    [SerializeField] List<GameObject> _guns;
    [SerializeField] List<GameObject> _arms;

    void Awake()
    {
        _inputs = new PlayerInputMap();
#if UNITY_EDITOR
        _inputs.Debugging.Unfocus.started += _ => FocusUnfocus();
        _inputs.Debugging.ChangeFramerate.started += _ => ChangeFramerate();
        _inputs.Debugging.ChangeTimeScale.started += _ => ChangeTimeScale();
        if (_guns != null)
        {

            _inputs.Debugging.Gun1.started += _ => Gun1();
            _inputs.Debugging.Gun2.started += _ => Gun2();
            _inputs.Debugging.Gun3.started += _ => Gun3();
            _inputs.Debugging.Gun4.started += _ => Gun4();
        }
        if (_arms != null)
        {
            _inputs.Debugging.Skill1.started += _ => Skill1();
            _inputs.Debugging.Skill2.started += _ => Skill2();
            _inputs.Debugging.Skill3.started += _ => Skill3();
            _inputs.Debugging.Skill4.started += _ => Skill4();
        }
#endif
    }

    #region Debug Equipping
    private void Skill4()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[3].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayArmEquip();
    }

    private void Skill3()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[2].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayArmEquip();
    }

    private void Skill2()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[1].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayArmEquip();
    }

    private void Skill1()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[0].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayArmEquip();
    }

    private void Gun4()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[3].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }

    private void Gun3()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[2].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }

    private void Gun2()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[1].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }

    private void Gun1()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[0].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    #endregion

    void Start()
    {
        // curseurs
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Framerate
        _is30fps = false;

#if UNITY_EDITOR
        _DebuggingCanvas.SetActive(true);
#endif
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

        if (_timescaleDebugUi)
            _timescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F1"));
    }

    void FocusUnfocus()
    {
        if (_isFocused)
        {
            _isFocused = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            _isFocused = true;
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

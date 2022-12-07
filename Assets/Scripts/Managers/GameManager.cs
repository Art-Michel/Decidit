using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : LocalManager<GameManager>
{
    float _slowMoT;
    float _slowMoInitialT;
    float _timeSpeed;

    #region Debug
    PlayerInputMap _inputs;
    [SerializeField] GameObject _DebuggingCanvas;
    [SerializeField] TextMeshProUGUI _timescaleDebugUi;
    bool _is30fps;
    [SerializeField] List<GameObject> _guns;
    [SerializeField] List<GameObject> _arms;

    private void DebugAwake()
    {
#if UNITY_EDITOR
        _inputs = new PlayerInputMap();
        _inputs.Debugging.ChangeFramerate.started += _ => DebugChangeFramerate();
        _inputs.Debugging.ChangeTimeScale.started += _ => DebugChangeTimeScale();
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

    private void DebugStart()
    {
#if UNITY_EDITOR
        if (_DebuggingCanvas)_DebuggingCanvas.SetActive(true);

        //Framerate
        _is30fps = false;

#endif
    }

    private void DebugChangeFramerate()
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

    private void DebugChangeTimeScale()
    {
        var direction = _inputs.Debugging.ChangeTimeScale.ReadValue<float>();

        if (Mathf.Sign(direction) > 0)
            Time.timeScale = Mathf.Clamp(Time.timeScale + .1f, 0.01f, 10);
        else if (Mathf.Sign(direction) < 0)
            Time.timeScale = Mathf.Clamp(Time.timeScale - .1f, 0.01f, 10);

        DisplayTimeScale();
    }

    private void DisplayTimeScale()
    {
        if (_timescaleDebugUi)
            _timescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F3"));
    }
    //Equipping
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

    protected override void Awake()
    {
        base.Awake();
        DebugAwake();
    }

    private void Start()
    {
        // curseurs
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DebugStart();
    }

    private void Update()
    {
        SlowMo();
    }

    public void StartSlowMo(float speed, float duration)
    {
        if (duration > _slowMoT)
        {
            _slowMoInitialT = duration;
            _slowMoT = duration;
        }
        if (speed < _timeSpeed)
            _timeSpeed = speed;
    }

    private void SlowMo()
    {
        if (_slowMoT > 0)
        {
            Time.timeScale = Mathf.Lerp(_timeSpeed, 1, Mathf.InverseLerp(_slowMoInitialT, 0, _slowMoT));
            DisplayTimeScale();

            _slowMoT -= Time.unscaledDeltaTime;
            if (_slowMoT < 0)
                StopSlowMo();
        }
    }

    private void StopSlowMo()
    {
        Time.timeScale = 1;
        _slowMoT = 0;
        DisplayTimeScale();
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

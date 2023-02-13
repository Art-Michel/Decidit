using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using NaughtyAttributes;

public class PlayerManager : LocalManager<PlayerManager>
{
    //Slow down time
    float _slowMoT;
    float _slowMoInitialT;
    float _timeSpeed;
    [SerializeField] TextMeshProUGUI _timescaleDebugUi;

    //Controller Rumble
    float _rumbleT;
    float _rumbleInitialT;
    float _rumbleLowFreqIntensity;
    float _rumbleHighFreqIntensity;
    bool _isRumbling;

    //Display framerate
    [SerializeField] GameObject _DebuggingCanvas;
    [SerializeField] TextMeshProUGUI _fps;
    float _delayFramerateCalculation;

    bool _isLockedAt30;

    float _timescaleBeforePausing;
    bool _isPaused;
    [Foldout("Things to disable on pause")]
    [SerializeField] GameObject _pauseMenu;
    [Foldout("Things to disable on pause")]
    [SerializeField] List<GameObject> _guns;
    [Foldout("Things to disable on pause")]
    [SerializeField] List<GameObject> _arms;
    [Foldout("Things to disable on pause")]
    [SerializeField] Volume _postProcessVolume;
    [Foldout("Things to disable on pause")]
    [SerializeField] GameObject _healthBar;

    PlayerInputMap _inputs;

    protected override void Awake()
    {
        base.Awake();
        _inputs = new PlayerInputMap();
        _inputs.Debugging.DisplayFramerate.started += _ => DisplayFramerate();
        _inputs.MenuNavigation.Pause.started += _ => PressPause();
        _inputs.Debugging.Lock.started += _ => LockFramerate();
    }

    private void Start()
    {
        // curseurs
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _isPaused = false;

        _isLockedAt30 = false;
    }

    private void Update()
    {
        SlowMo();
        Rumble();

        if (_fps.enabled)
            UpdateFramerate();
    }

    private void LockFramerate()
    {
        if (_isPaused)
            return;
        if (_isLockedAt30)
        {
            _isLockedAt30 = false;
            Application.targetFrameRate = 0;
        }
        else
        {
            _isLockedAt30 = true;
            Application.targetFrameRate = 60;
        }
    }

    private void DisplayTimeScale()
    {
        if (_timescaleDebugUi)
            _timescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F3"));
    }

    #region Equipping
    public void Skill4()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[3].SetActive(true);
    }
    public void Skill3()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[2].SetActive(true);
    }
    public void Skill2()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[1].SetActive(true);
    }
    public void Skill1()
    {
        foreach (GameObject arm in _arms)
            arm.SetActive(false);
        _arms[0].SetActive(true);
    }
    public void Gun4()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[3].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void Gun3()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[2].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void Gun2()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[1].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void Gun1()
    {
        foreach (GameObject gun in _guns)
            gun.SetActive(false);
        _guns[0].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    #endregion

    #region Pause
    private void PressPause()
    {
        if (_isPaused)
            Unpause();
        else
            Pause();
    }

    public void Pause()
    {
        //timescale
        _timescaleBeforePausing = Time.timeScale;
        Time.timeScale = 0f;

        //Enable Pause 
        _isPaused = true;
        _pauseMenu.SetActive(true);
        MenuManager.Instance.gameObject.SetActive(true);

        //blur
        _postProcessVolume.enabled = true;

        //Disable everything
        Player.Instance.enabled = false;
        foreach (GameObject gun in _guns)
            gun.GetComponent<Revolver>().enabled = false;
        foreach (GameObject arm in _arms)
            arm.GetComponent<Arm>().enabled = false;
        _healthBar.SetActive(false);

        //disable rumble
        StopRumbling();

        //cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Unpause()
    {
        //timescale
        Time.timeScale = _timescaleBeforePausing;

        //enable pause
        _isPaused = false;
        _pauseMenu.SetActive(false);
        MenuManager.Instance.gameObject.SetActive(false);

        //blur
        _postProcessVolume.enabled = false;

        //re enable everything
        Player.Instance.enabled = true;
        foreach (GameObject gun in _guns)
            gun.GetComponent<Revolver>().enabled = true;
        foreach (GameObject arm in _arms)
            arm.GetComponent<Arm>().enabled = true;
        _healthBar.SetActive(true);

        //cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region Framerate Displayer
    private void DisplayFramerate()
    {
        _fps.enabled = !_fps.enabled;
    }

    private void UpdateFramerate()
    {
        if (_delayFramerateCalculation <= 0)
        {
            _fps.text = (1 / Time.unscaledDeltaTime).ToString("F1");
            _delayFramerateCalculation = 0.05f;
        }
        else
            _delayFramerateCalculation -= Time.deltaTime;
    }
    #endregion

    #region Slow mo
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
        if (_slowMoT > 0 || !_isPaused)
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
    #endregion

    #region Rumble
    public void StartRumbling(float lowFreqStrength, float highFreqStrength, float duration)
    {
        if (duration > _rumbleT)
        {
            _rumbleInitialT = duration;
            _rumbleT = duration;
        }

        if (lowFreqStrength > _rumbleLowFreqIntensity)
        {
            _rumbleLowFreqIntensity = lowFreqStrength;
        }
        if (highFreqStrength > _rumbleHighFreqIntensity)
        {
            _rumbleHighFreqIntensity = highFreqStrength;
        }


        if (Gamepad.current != null)
            Gamepad.current.ResumeHaptics();
        _isRumbling = true;
    }

    private void Rumble()
    {
        if (_isRumbling)
        {
            float low = _rumbleLowFreqIntensity * Mathf.InverseLerp(0, _rumbleInitialT, _rumbleT);
            float high = _rumbleHighFreqIntensity * Mathf.InverseLerp(0, _rumbleInitialT, _rumbleT);
            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(low, high);

            _rumbleT -= Time.deltaTime;
            if (_rumbleT <= 0.1f)
                StopRumbling();
        }
    }

    private void StopRumbling()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
            Gamepad.current.PauseHaptics();
        }
        _rumbleHighFreqIntensity = 0f;
        _rumbleLowFreqIntensity = 0f;
        _rumbleInitialT = 0f;
        _rumbleT = 0;
        _isRumbling = false;
    }
    #endregion

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

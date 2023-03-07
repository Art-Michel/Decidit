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
    [Foldout("Debugging")]
    [SerializeField] TextMeshProUGUI _timescaleDebugUi;

    //Controller Rumble
    float _rumbleT;
    float _rumbleInitialT;
    float _rumbleLowFreqIntensity;
    float _rumbleHighFreqIntensity;
    bool _isRumbling;

    //Display framerate
    [Foldout("Debugging")]
    [SerializeField] GameObject _DebuggingCanvas;
    [Foldout("Debugging")]
    [SerializeField] TextMeshProUGUI _fps;
    float _delayFramerateCalculation;

    //Lock framerate
    bool _isLockedAt60;

    //Pausing
    public bool _canPause = true;
    float _timescaleBeforePausing;
    bool _isPaused;
    [Foldout("Menu")]
    // [SerializeField] GameObject _menu;

    //Death variables
    bool _isDying;
    float _dieT;
    [Foldout("Death")]
    [SerializeField] float _deathDuration = 1f;

    //Victory variables

    //StopGame stuff
    public List<GameObject> Guns;
    public List<GameObject> Arms;

    //Altar usage
    Altar _currentAltar = null;

    PlayerInputMap _inputs;

    protected override void Awake()
    {
        base.Awake();
        _inputs = new PlayerInputMap();
        _inputs.Debugging.DisplayFramerate.started += _ => DisplayFramerate();
        _inputs.MenuNavigation.Pause.started += _ => PressPause();
        _inputs.Debugging.Lock.started += _ => LockFramerate();
        _inputs.MenuNavigation.anyButton.started += _ => SwitchToController();
        _inputs.MenuNavigation.moveMouse.started += _ => SwitchToMouse();
    }

    private void Start()
    {
        _isPaused = false;
        MenuManager.Instance.StopMenuing();
        _isLockedAt60 = false;
    }

    private void Update()
    {
        SlowMo();
        Rumble();

        if (_isDying)
            Die();

        if (_fps.enabled)
            UpdateFramerate();
    }

    private void LockFramerate()
    {
        if (_isPaused)
            return;
        if (_isLockedAt60)
        {
            _isLockedAt60 = false;
            Application.targetFrameRate = 0;
        }
        else
        {
            _isLockedAt60 = true;
            Application.targetFrameRate = 60;
        }
    }

    private void DisplayTimeScale()
    {
        if (_timescaleDebugUi)
            _timescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F3"));
    }

    #region Equipping

    public void AltarEquipGun2()
    {
        ForceEquipGun2();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseAGun = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipGun3()
    {
        ForceEquipGun3();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseAGun = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipGun4()
    {
        ForceEquipGun4();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseAGun = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipSkill2()
    {
        ForceEquipSkill2();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseASkill = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipSkill3()
    {
        ForceEquipSkill3();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseASkill = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipSkill4()
    {
        ForceEquipSkill4();
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseASkill = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }
    #endregion

    #region AltarMenuing
    public void StartAltarMenuing(Altar altar)
    {
        Player.Instance.PlayerHealth.IsInvulnerable = true;
        Player.Instance.AllowMovement(false);
        Player.Instance.KillMomentum();
        Player.Instance.CharaCon.enabled = false;
        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.OpenSubmenu(altar.MenuToOpen);
        _currentAltar = altar;
        _canPause = false;

        if (DungeonGenerator.Instance != null)
        {
            if (DungeonGenerator.Instance.ChoseAGun)
                MenuManager.Instance.GreyOutItem(altar.MenuToOpen, 0);
            if (DungeonGenerator.Instance.ChoseASkill)
                MenuManager.Instance.GreyOutItem(altar.MenuToOpen, 1);
        }
    }

    public void StopAltarMenuing()
    {
        _currentAltar = null;
        MenuManager.Instance.StopMenuing();
        Player.Instance.PlayerHealth.IsInvulnerable = false;
        Player.Instance.AllowMovement(true);
        Player.Instance.CharaCon.enabled = true;
        Player.Instance.ForceRotation(Player.Instance.Head);
        _canPause = true;
    }
    #endregion

    #region ForceEquipping
    public void ForceEquipSkill4()
    {
        foreach (GameObject arm in Arms)
            arm.SetActive(false);
        Arms[3].SetActive(true);
    }
    public void ForceEquipSkill3()
    {
        foreach (GameObject arm in Arms)
            arm.SetActive(false);
        Arms[2].SetActive(true);
    }
    public void ForceEquipSkill2()
    {
        foreach (GameObject arm in Arms)
            arm.SetActive(false);
        Arms[1].SetActive(true);
    }
    public void ForceEquipSkill1()
    {
        foreach (GameObject arm in Arms)
            arm.SetActive(false);
        Arms[0].SetActive(true);
    }
    public void ForceEquipGun4()
    {
        foreach (GameObject gun in Guns)
            gun.SetActive(false);
        Guns[3].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void ForceEquipGun3()
    {
        foreach (GameObject gun in Guns)
            gun.SetActive(false);
        Guns[2].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void ForceEquipGun2()
    {
        foreach (GameObject gun in Guns)
            gun.SetActive(false);
        Guns[1].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    public void ForceEquipGun1()
    {
        foreach (GameObject gun in Guns)
            gun.SetActive(false);
        Guns[0].SetActive(true);
        PlaceHolderSoundManager.Instance.PlayWeaponEquip();
    }
    #endregion

    #region Pause unpause
    private void PressPause()
    {
        if (!_canPause)
            return;

        if (_isPaused)
            Unpause();
        else
            Pause();
    }

    public void Pause()
    {
        _isPaused = true;
        // _menu.SetActive(true);

        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.OpenMain();
        StopGame();
    }

    public void Unpause()
    {
        _isPaused = false;
        // _menu.SetActive(false);

        MenuManager.Instance.StopMenuing();
        ResumeGame();
    }

    private void StopGame()
    {
        //timescale
        _timescaleBeforePausing = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        if (_timescaleBeforePausing != 0)
            Time.timeScale = _timescaleBeforePausing;
        else
            Time.timeScale = 1;
    }

    public void StartDying()
    {
        if (_isDying)
            return;

        _isDying = true;
        _dieT = 0f;
        StopRumbling();
        StopSlowMo();
        _canPause = false;
    }

    private void Die()
    {
        _dieT += Time.unscaledDeltaTime;

        Time.timeScale = Mathf.Lerp(0, 1, Mathf.InverseLerp(_deathDuration, 0, _dieT));
        //TODO reenable StartRumbling(1, 1, _dieT);

        if (_dieT >= _deathDuration)
            Dead();
    }

    private void Dead()
    {
        _isDying = false;
        // _menu.SetActive(true);
        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.OpenDeath();
        StopGame();
    }

    public void OnPlayerWin()
    {
        // _menu.SetActive(true);
        _canPause = false;
        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.OpenWin();
        StopGame();
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
            ////SoundManager.Instance.PlaySound("event:/SFX_Environement/SlowMo", 5f);
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

    public void StopRumbling()
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

    #region Change current device
    private void SwitchToMouse()
    {
        if (MenuManager.Instance.CurrentDevice == MenuManager.Devices.Mouse)
            return;
        MenuManager.Instance.CurrentDevice = MenuManager.Devices.Mouse;
    }

    private void SwitchToController()
    {
        if (MenuManager.Instance.CurrentDevice == MenuManager.Devices.Controller)
            return;
        MenuManager.Instance.CurrentDevice = MenuManager.Devices.Controller;
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

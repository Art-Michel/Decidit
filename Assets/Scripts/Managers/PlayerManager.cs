using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using NaughtyAttributes;
using CameraShake;
using UnityEngine.UI;

public class PlayerManager : LocalManager<PlayerManager>
{
    [Foldout("References")]
    [SerializeField] Volume _playerVolume;
    [Foldout("References")]
    [SerializeField] Image _flash;
    ColorAdjustments _colorPP;

    //Stop time
    bool _timeStopped = true;
    float _timeStopT;

    //Slow down time
    float _slowMoT;
    float _slowMoInitialT;
    float _timeSpeed;
    [Foldout("Debugging")]
    [SerializeField] TextMeshProUGUI _timescaleDebugUi;

    //Flashing
    float _flashT;
    float _flashInitialT;
    float _flashStrength;

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
    // [Foldout("Menu")]
    // [SerializeField] GameObject _menu;

    //Death variables
    public bool _isDying { get; private set; }
    private bool _isDead;
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
    private bool _forceSlowMo;
    [SerializeField] private AnimationCurve _flashCurve;

    protected override void Awake()
    {
        base.Awake();
        _inputs = new PlayerInputMap();
        _inputs.Debugging.DisplayFramerate.started += _ => DisplayFramerate();
        _inputs.MenuNavigation.Pause.started += _ => PressPause();
        _inputs.Debugging.Lock.started += _ => LockFramerate();
        _inputs.Debugging.Slow.started += _ => SlowDownTime();
        _inputs.Debugging.DisplayDebug.started += _ => ShowDebug();
        _inputs.Debugging.HideUi.started += _ => HideUi();
        _inputs.Debugging.HideGuns.started += _ => HideGuns();
        _inputs.Debugging.Teleport.started += _ => Teleport();
        _inputs.Debugging.ForceInvul.started += _ => ForceInvul();
        // _inputs.MenuNavigation.anyButton.started += _ => SwitchToController();
        // _inputs.MenuNavigation.moveMouse.started += _ => SwitchToMouse();
    }

    private void ShowDebug()
    {
        _DebuggingCanvas.SetActive(!_DebuggingCanvas.activeInHierarchy);
    }

    private bool _uiVisible = true;
    private void HideUi()
    {
        if (_uiVisible)
            foreach (Canvas canva in GameObject.FindObjectsOfType<Canvas>())
            {
                canva.enabled = false;
                _uiVisible = false;
            }
        else
            foreach (Canvas canva in GameObject.FindObjectsOfType<Canvas>())
            {
                canva.enabled = true;
                _uiVisible = true;
            }
    }

    private bool _gunsVisible = true;
    private void HideGuns()
    {
        if (_gunsVisible)
        {
            Player.Instance.CurrentGun.Animator.gameObject.SetActive(false);
            Player.Instance.CurrentArm.Animator.gameObject.SetActive(false);
            _gunsVisible = false;
        }
        else
        {
            Player.Instance.CurrentGun.Animator.gameObject.SetActive(true);
            Player.Instance.CurrentArm.Animator.gameObject.SetActive(true);
            _gunsVisible = true;
        }

    }

    private void Start()
    {
        _isPaused = false;
        _playerVolume.profile.TryGet<ColorAdjustments>(out ColorAdjustments colorPP);
        _colorPP = colorPP;
        _colorPP.saturation.overrideState = true;
        MenuManager.Instance.StopMenuing();
        ForceEquipGun(0);
        _isLockedAt60 = false;
        _currentColor = _baseGunHitColor;
    }

    private void Update()
    {
        SlowMo();
        Rumble();
        Flash();
        DisplayTimeScale();
        UpdateHitmarker();

        if (_timeStopped)
        {
            _timeStopT -= Time.unscaledDeltaTime;
            if (_timeStopT <= 0.0f)
            {
                _timeStopped = false;
                Time.timeScale = 1.0f;
            }
        }

        if (_isDying)
            Die();

        if (_fps.enabled)
            UpdateFramerate();
    }

    private void SlowDownTime()
    {
        if (Time.timeScale == 1.0f)
        {
            _forceSlowMo = true;
            Time.timeScale = 0.1f;
        }
        else
        {
            _forceSlowMo = false;
            Time.timeScale = 1.0f;
        }
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
            Application.targetFrameRate = 30;
        }
    }

    private void DisplayTimeScale()
    {
        if (_timescaleDebugUi)
            _timescaleDebugUi.text = ("TimeScale: " + Time.timeScale.ToString("F3"));
    }

    public void HitShake(BounceShake.Params shake)
    {
        //prendre en argument un shake, les faire au cas par cas
        Player.Instance.StartBounceShake(shake, transform.position);
    }

    public void HitStop(float duration)
    {
        // Time.timeScale = 0.0f;
        // _timeStopT = duration;
        // _timeStopped = true;
    }

    #region Equipping

    public void AltarEquipGun(int i)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/ChoosingChant", 1f, gameObject);

        ForceEquipGun(i);
        if (DungeonGenerator.Instance != null)
            DungeonGenerator.Instance.ChoseAGun = true;

        _currentAltar.TurnOff();
        StopAltarMenuing();
    }

    public void AltarEquipSkill(int i)
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Chants/ChoosingChant", 1f, gameObject);
        ForceEquipSkill(i);
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
        Player.Instance.KillGravity();
        Player.Instance.KillMomentum();
        Player.Instance.CharaCon.enabled = false;
        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.OpenSubmenu(altar.MenuToOpen, false);
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

    public void ForceEquipSkill(int i)
    {
        foreach (GameObject arm in Arms)
            arm.SetActive(false);
        Arms[i].SetActive(true);
        Player.Instance.CurrentArm = Arms[i].GetComponent<Arm>();
        Player.Instance.InspectArm();
    }
    public void ForceEquipGun(int i)
    {
        foreach (GameObject gun in Guns)
            gun.SetActive(false);
        Guns[i].SetActive(true);
        SetHitmarkerColor(_baseGunHitColor);
        Guns[i].GetComponent<Revolver>().InitialPos = Guns[i].transform.localPosition;
        Player.Instance.CurrentGun = Guns[i].GetComponent<Revolver>();
        Player.Instance.InspectWeapon();
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

    public void RechargeEverything()
    {
        // foreach (GameObject revolver in Guns)
        // {
        //     if (revolver.activeInHierarchy)
        //         revolver.GetComponent<Revolver>().Reloaded();
        // }
        foreach (GameObject arm in Arms)
        {
            if (arm.activeInHierarchy)
                arm.GetComponent<Arm>().ForceRefill();
        }
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
        PlayerHealth.Instance.IsInvulnerable = true;
        StopRumbling();
        StopSlowMo();
        StopFlash();
        _canPause = false;
    }

    public void CancelDeath()
    {
        PlayerHealth.Instance.IsInvulnerable = false;
        _isDying = false;
        _dieT = 0f;
        StopFlash();
        StopRumbling();
        StopSlowMo();
        _colorPP.saturation.value = 0;
        _canPause = true;
        //TODO Lucas un son quand on se ressuscite
    }

    private void Die()
    {
        _dieT += Time.unscaledDeltaTime;

        _colorPP.saturation.value = Mathf.Lerp(-100, 0, Mathf.InverseLerp(_deathDuration, 0, _dieT));
        Time.timeScale = Mathf.Lerp(0, 1, Mathf.InverseLerp(_deathDuration, 0, _dieT));

        if (_dieT >= _deathDuration)
            Dead();
    }

    private void Dead()
    {
        _isDying = false;
        _isDead = true;
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
        TimerManager.Instance.SaveTimer();
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
        if (_timeStopped || _isPaused && _forceSlowMo)
            return;

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
        if (_timeStopped || _slowMoT < 0 || _isPaused && _forceSlowMo)
            return;

        ////SoundManager.Instance.PlaySound("event:/SFX_Environement/SlowMo", 5f);
        Time.timeScale = Mathf.Lerp(_timeSpeed, 1, Mathf.InverseLerp(_slowMoInitialT, 0, _slowMoT));

        _slowMoT -= Time.unscaledDeltaTime;
        if (_slowMoT < 0 && !_isDead)
            StopSlowMo();
    }

    private void StopSlowMo()
    {
        Time.timeScale = 1;
        _slowMoT = -1;
    }
    #endregion

    #region Flash
    public void StartFlash(float duration, float strength)
    {
        if (_timeStopped || _isPaused && _forceSlowMo)
            return;

        if (duration > _flashT)
        {
            _flashInitialT = duration;
            _flashT = duration;
        }

        _flashStrength = strength;
    }

    private void Flash()
    {
        if (_timeStopped || _flashT < 0 || _isPaused && _forceSlowMo)
            return;

        float alpha = _flashCurve.Evaluate(Mathf.InverseLerp(_flashInitialT, 0, _flashT)) * _flashStrength;
        _flash.color = new Color(1.0f, 1.0f, 1.0f, alpha);

        _flashT -= Time.deltaTime;
        if (_flashT < 0 && !_isDead)
            StopFlash();
    }

    private void StopFlash()
    {
        _flash.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _flashT = -1;
    }
    #endregion

    #region Rumble
    public void StartRumbling(float lowFreqStrength, float highFreqStrength, float duration)
    {
        if (MenuManager.Instance.CurrentDevice != MenuManager.Devices.Controller)
            return;

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
    // private void SwitchToMouse()
    // {
    //     if (MenuManager.Instance.CurrentDevice == MenuManager.Devices.Mouse)
    //         return;
    //     MenuManager.Instance.SwitchToMouse();
    // }

    // private void SwitchToController()
    // {
    //     if (MenuManager.Instance.CurrentDevice == MenuManager.Devices.Controller)
    //         return;
    //     MenuManager.Instance.SwitchToController();
    // }

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

    #region Push player towards spawn
    public void PushPlayerTowardsSpawn()
    {
        Debug.Log("player is stuck");
        if (DungeonGenerator.Instance)
        {
            Vector3 dir = DungeonGenerator.Instance.GetRoom().Entry.transform.position - Player.Instance.transform.position;
            Player.Instance.transform.position += dir.normalized;
        }
    }
    #endregion

    #region Hitmarker
    [Foldout("Hitmarker")]
    [SerializeField] private AnimationCurve _hitmarkerCurve;
    [Foldout("Hitmarker")]
    [SerializeField] private CanvasGroup _hitmarkerCanvasGroup;
    [Foldout("Hitmarker")]
    [SerializeField] private float _hitmarkerSpeed;
    [Foldout("Hitmarker")]
    [SerializeField] private Image[] _hitmarkers;
    [Foldout("Hitmarker")]
    [SerializeField] private float _hitmarkerMaxAlpha;
    [Foldout("Hitmarker")]
    [SerializeField] private float _critHitmarkerMaxAlpha;
    [Foldout("Hitmarker")]
    [SerializeField] private Color _fugueGunHitColor;
    [Foldout("Hitmarker")]
    [SerializeField] private Color _museGunHitColor;
    [Foldout("Hitmarker")]
    [SerializeField] private Color _eylauGunHitColor;
    [Foldout("Hitmarker")]
    [SerializeField] private Color _baseGunHitColor;
    private float _currentHitmarkerMaxAlpha;
    private Color _currentColor;

    private float _hitmarkerT = 1.0f;

    public void SetHitmarkerColor(Color color)
    {
        _currentColor = color;
    }

    public void Hitmarker()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/HitMarker", .2f, gameObject);
        _hitmarkerT = 0.0f;
        _currentHitmarkerMaxAlpha = _hitmarkerMaxAlpha;
        foreach (Image hitmarker in _hitmarkers)
            hitmarker.color = _currentColor;
    }

    public void Crithitmarker()
    {
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/HitMarkerHead", .8f, gameObject);
        _hitmarkerT = 0.0f;
        _currentHitmarkerMaxAlpha = _critHitmarkerMaxAlpha;
        foreach (Image hitmarker in _hitmarkers)
            hitmarker.color = Color.red;
    }

    private void UpdateHitmarker()
    {
        if (_hitmarkerT >= 1.0f)
            return;
        _hitmarkerT += Time.deltaTime * _hitmarkerSpeed;
        _hitmarkerCanvasGroup.alpha = _hitmarkerCurve.Evaluate(_hitmarkerT) * _currentHitmarkerMaxAlpha;
    }
    #endregion

    #region DebugTeleport
    private void Teleport()
    {

    }
    #endregion

    #region DebugInvul

    [Foldout("Debugging")]
    [SerializeField] TextMeshProUGUI _invulPrompt;
    private void ForceInvul()
    {
        PlayerHealth.Instance._isDebugInvulnerable = !PlayerHealth.Instance._isDebugInvulnerable;
        _invulPrompt.enabled = PlayerHealth.Instance._isDebugInvulnerable;
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using UnityEngine.VFX;
using System;

public class Revolver : MonoBehaviour
{
    #region References Decleration
    [Foldout("References")]
    [SerializeField] private TextMeshProUGUI _ammoCountText;
    [Foldout("References")]
    [SerializeField] protected Transform _canonPosition;
    [Foldout("References")]
    [SerializeField] protected GameObject _ui;
    [Foldout("References")]
    [SerializeField] protected VFX_Particle _muzzleFlash;
    [Foldout("References")]
    [SerializeField] protected Image _reloadingWarning;
    [Foldout("References")]
    [SerializeField] protected Pooler _trailVfxPooler;
    [Foldout("References")]
    [SerializeField] protected Pooler _impactVfxPooler;
    [Foldout("References")]
    [SerializeField] protected Pooler _fleshSplashVfxPooler;

    private PlayerInputMap _inputs;
    private RevolverFSM _fsm;
    protected Transform _camera;

    #endregion

    #region Stats Decleration
    [Foldout("Stats")]
    [SerializeField] protected LayerMask _mask;
    [Foldout("Stats")]
    [SerializeField] protected float _recoilTime = .3f;
    [Foldout("Stats")]
    [SerializeField][Tooltip("total reload animation duration")] private float _reloadMaxTime = 1f;
    [Foldout("Stats")]
    [SerializeField][Tooltip("time before which you can cancel reloading")] private float _reloadMinTime = 0.9f;
    [Foldout("Stats")]
    [SerializeField] private int _maxAmmo = 6;
    [Foldout("Stats")]
    [SerializeField] protected float _shootShakeIntensity;
    [Foldout("Stats")]
    [SerializeField] protected float _shootShakeDuration;

    [Foldout("Other")]
    [SerializeField] Color _fullAmmoColor;
    [Foldout("Other")]
    [SerializeField] Color _lowAmmoColor;
    [Foldout("Other")]
    [SerializeField] Color _noAmmoColor;
    [Foldout("Other")]
    [SerializeField] private float _smoothness = 10;
    [Foldout("Other")]
    [SerializeField] private float _mouseSwayAmountX = -.2f;
    [Foldout("Other")]
    [SerializeField] private float _mouseSwayAmountY = -.1f;
    [Foldout("Other")]
    [SerializeField] private float _controllerSwayAmountX = -5f;
    [Foldout("Other")]
    [SerializeField] private float _controllerSwayAmountY = -3f;


    protected Vector3 _currentlyAimedAt;
    protected float _recoilT;
    private float _reloadT;
    private int _ammo;
    private bool _reloadBuffered;
    #endregion

    void Awake()
    {
        _fsm = GetComponent<RevolverFSM>();
        _camera = Camera.main.transform;
        _inputs = new PlayerInputMap();
        _inputs.Actions.Shoot.started += _ => PressShoot();
        _inputs.Actions.Reload.started += _ => PressReload();
    }

    void Start()
    {
        Reloaded();
    }

    void Update()
    {
        //State update
        if (_fsm.currentState != null)
            _fsm.currentState.StateUpdate();

        CheckLookedAt();
        //Debugging();
    }

    //     #region Debugging
    //     void Debugging()
    //     {
    // #if UNITY_EDITOR
    //         DebugDisplayGunState();
    // #endif
    //     }

    //     public void DebugDisplayGunState()
    //     {
    //         if (_debugStateText && _fsm.currentState != null)
    //             _debugStateText.text = ("Revolver state: " + _fsm.currentState.Name);
    //     }
    //     #endregion

    #region Swaying
    public void StartSwaying()
    {

    }

    public void Sway()
    {
        float x = _inputs.Camera.Rotate.ReadValue<Vector2>().x * _mouseSwayAmountX;
        float y = _inputs.Camera.Rotate.ReadValue<Vector2>().y * _mouseSwayAmountY;
        x += _inputs.Camera.RotateX.ReadValue<float>() * _controllerSwayAmountX;
        y += _inputs.Camera.RotateY.ReadValue<float>() * _controllerSwayAmountY;

        Quaternion rotationX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion targetRot = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, _smoothness * Time.deltaTime);
    }

    public void StopSwaying()
    {
        transform.localRotation = Quaternion.identity;
    }
    #endregion

    #region Aiming
    //raycast forward to aim gun in that direction
    public void CheckLookedAt()
    {
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, 9999999999f, _mask))
            _currentlyAimedAt = hit.point;
        else
            _currentlyAimedAt = _camera.forward * 9999999999f;
    }
    #endregion

    #region Shooting
    private void PressShoot()
    {
        if (_fsm)
        {
            if ((_fsm.currentState.Name == RevolverStateList.IDLE || _fsm.currentState.Name == RevolverStateList.RELOADING) && _ammo > 0)
                _fsm.ChangeState(RevolverStateList.SHOOTING);
        }
    }

    public void CheckBuffer()
    {
        if (_inputs.Actions.Shoot.IsPressed())
            PressShoot();
        if (_reloadBuffered)
            PressReload();

        _reloadBuffered = false;
    }

    public virtual void Shoot()
    {

    }

    public virtual void StartRecoil()
    {
        _recoilT = _recoilTime;
    }

    public void Recoiling()
    {
        _recoilT -= Time.deltaTime;
        if (_recoilT <= 0)
        {
            if (_ammo > 0)
                _fsm.ChangeState(RevolverStateList.IDLE);
            else
                _fsm.ChangeState(RevolverStateList.RELOADING);
        }
    }

    public virtual void UpdateChargeLevel()
    {
        Debug.Log("mario");
    }
    public virtual void ResetChargeLevel()
    {

    }
    #endregion

    #region Ammo
    public virtual void LowerAmmoCount()
    {
        _ammo--;
        DisplayAmmo();
    }

    //register input then enter reloading state
    private void PressReload()
    {
        if (_fsm.currentState.Name == RevolverStateList.IDLE && _ammo < _maxAmmo)
            _fsm.ChangeState(RevolverStateList.RELOADING);
        else if (_fsm.currentState.Name == RevolverStateList.SHOOTING && _ammo < _maxAmmo)
            _reloadBuffered = true;
    }

    // Initiate reloading
    public void StartReload()
    {
        _reloadT = _reloadMaxTime;
        _reloadingWarning.fillAmount = 0f;
        _reloadingWarning.enabled = true;
        ////PlaceHolderSoundManager.Instance.PlayReload();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/Realod", 1f);//TODO Art
    }

    // update reloading state
    public void Reloading()
    {
        _reloadT -= Time.deltaTime;
        _reloadingWarning.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(_reloadMaxTime, 0 + (_reloadMaxTime - _reloadMinTime), _reloadT));
        if (_reloadT <= _reloadMaxTime - _reloadMinTime && _ammo < _maxAmmo)
        {
            Reloaded();
        }
        if (_reloadT <= 0)
        {
            _fsm.ChangeState(RevolverStateList.IDLE);
        }
    }

    //Actually refill ammo
    private void Reloaded()
    {
        _ammo = _maxAmmo;
        //TODO lucas PlaceHolderSoundManager.Instance.PlayReloaded();
        DisplayAmmo();
    }

    public void DisableReloadUI()
    {
        _reloadingWarning.enabled = false;
        _reloadingWarning.fillAmount = 0f;
    }

    private void DisplayAmmo()
    {
        if (_ammoCountText)
        {
            _ammoCountText.text = _ammo.ToString() + "/" + _maxAmmo.ToString();
            //Color
            if (_ammo <= 1)
            {
                if (_ammo == 1)
                    _ammoCountText.color = _lowAmmoColor;
                else
                {
                    //TODO Lucas PlaceHolderSoundManager.Instance.PlayLastBulletClick();
                    _ammoCountText.color = _noAmmoColor;
                }
            }
            else _ammoCountText.color = _fullAmmoColor;

        }

        else
            Debug.LogError("AmmoCount UI Text unassigned.");
    }
    #endregion

    #region Enable Disable
    protected virtual void OnEnable()
    {
        if (PlaceHolderSoundManager.Instance != null)
            Reloaded();
        _inputs.Enable();
        _ui.SetActive(true);
    }

    protected virtual void OnDisable()
    {
        _inputs.Disable();
        _ui.SetActive(false);
    }
    #endregion
}
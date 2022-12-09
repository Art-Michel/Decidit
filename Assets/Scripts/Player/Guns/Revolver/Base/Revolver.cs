using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class Revolver : MonoBehaviour
{
    #region References Decleration
    [Header("References")]
    [SerializeField] TextMeshProUGUI _debugStateText;
    [SerializeField] TextMeshProUGUI _ammoCountText;
    [SerializeField] protected Transform _canon;
    PlayerInputMap _inputs;
    RevolverFSM _fsm;
    protected Transform _camera;

    #endregion

    #region Stats Decleration
    [Header("Stats")]
    [Tooltip("No use if not a hitscan weapon")][SerializeField] int _hitscanDamage;
    [SerializeField] float _recoilTime = .3f;
    [SerializeField] float _reloadMaxTime = 1f;
    [SerializeField] float _reloadMinTime = 0.9f;
    [SerializeField] int _maxAmmo = 6;
    [SerializeField] float _maxRange = 50f;
    [SerializeField] protected float _shootShakeIntensity;
    [SerializeField] protected float _shootShakeDuration;
    [SerializeField] LayerMask _mask;

    protected Vector3 _currentlyAimedAt;
    float _recoilT;
    float _reloadT;
    int _ammo;
    bool _reloadBuffered;
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

        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, Mathf.Infinity, _mask))
            _currentlyAimedAt = hit.point;
        else
            _currentlyAimedAt = _camera.forward * 1000;

        //Debugging
        Debugging();
    }

    #region Debugging
    void Debugging()
    {
#if UNITY_EDITOR
        DebugDisplayGunState();
#endif
    }

    public void DebugDisplayGunState()
    {
        if (_debugStateText && _fsm.currentState != null)
            _debugStateText.text = ("Revolver state: " + _fsm.currentState.Name);
    }
    #endregion

    #region Shooting
    private void PressShoot()
    {
        if ((_fsm.currentState.Name == RevolverStateList.IDLE || _fsm.currentState.Name == RevolverStateList.RELOADING) && _ammo > 0)
            _fsm.ChangeState(RevolverStateList.SHOOTING);
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
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _maxRange, _mask))
        {
            if (hit.transform.parent.TryGetComponent<Health>(out Health health))
            {
                if (hit.transform.CompareTag("WeakHurtbox"))
                    (health as EnemyHealth).TakeCriticalDamage(_hitscanDamage, hit.point, hit.normal);
                else
                    (health as EnemyHealth).TakeDamage(_hitscanDamage, hit.point, hit.normal);
            }
        }

        PlaceHolderSoundManager.Instance.PlayRevolverShot();
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
    }

    public void StartRecoil()
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
    #endregion

    #region Ammo
    public void LowerAmmoCount()
    {
        _ammo--;
        DisplayAmmo();
    }

    private void PressReload()
    {
        if (_fsm.currentState.Name == RevolverStateList.IDLE && _ammo < _maxAmmo)
            _fsm.ChangeState(RevolverStateList.RELOADING);
        else if (_fsm.currentState.Name == RevolverStateList.SHOOTING && _ammo < _maxAmmo)
            _reloadBuffered = true;
    }

    public void StartReload()
    {
        _reloadT = _reloadMaxTime;
        PlaceHolderSoundManager.Instance.PlayReload();
    }

    public void Reloading()
    {
        _reloadT -= Time.deltaTime;
        if (_reloadT <= _reloadMaxTime - _reloadMinTime && _ammo < _maxAmmo)
        {
            Reloaded();
        }
        if (_reloadT <= 0)
        {
            _fsm.ChangeState(RevolverStateList.IDLE);
        }
    }

    private void Reloaded()
    {
        _ammo = _maxAmmo;
        PlaceHolderSoundManager.Instance.PlayReloaded();
        DisplayAmmo();
    }

    private void DisplayAmmo()
    {
        if (_ammoCountText)
            _ammoCountText.text = _ammo.ToString() + "/" + _maxAmmo.ToString();
        else
            Debug.LogError("AmmoCount UI Text unassigned.");
    }
    #endregion

    #region Enable Disable Inputs
    void OnEnable()
    {
        Reloaded();
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}
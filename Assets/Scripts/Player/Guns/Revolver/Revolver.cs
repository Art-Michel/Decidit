using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Revolver : MonoBehaviour
{
    #region References Decleration
    [Header("References")]
    [SerializeField] TextMeshProUGUI _debugStateText;
    [SerializeField] TextMeshProUGUI _ammoCountText;
    PlayerInputMap _inputs;
    RevolverFSM _fsm;

    #endregion

    #region Stats Decleration
    [Header("Stats")]
    [SerializeField] float _recoilTime = .3f;
    [SerializeField] float _reloadMaxTime = 1f;
    [SerializeField] float _reloadMinTime = 0.9f;
    [SerializeField] int _maxAmmo = 6;

    float _recoilT;
    float _reloadT;
    int _ammo;
    bool _reloadBuffered;
    #endregion 

    void Awake()
    {
        _fsm = GetComponent<RevolverFSM>();
        _inputs = new PlayerInputMap();
        _inputs.Shooting.Shoot.started += _ => PressShoot();
        _inputs.Shooting.Reload.started += _ => PressReload();
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

        //Debugging
        Debugging();
    }

    #region Debugging
    void Debugging()
    {
#if UNITY_EDITOR
        if (_debugStateText && _fsm.currentState != null)
            _debugStateText.text = ("Revolver state: " + _fsm.currentState.Name);
#endif
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
        if (_inputs.Shooting.Shoot.IsPressed())
            PressShoot();
        if (_reloadBuffered)
            PressReload();

        _reloadBuffered = false;
    }

    public void BaseShoot()
    {
        //Raycast Here
        //SFX
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
    }

    public void Reloading()
    {
        _reloadT -= Time.deltaTime;
        if (_reloadT <= _reloadMaxTime - _reloadMinTime)
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
        DisplayAmmo();
    }

    private void DisplayAmmo()
    {
        _ammoCountText.text = _ammo.ToString() + "/" + _maxAmmo.ToString();
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
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class Arm : MonoBehaviour
{
    #region References Decleration
    [Foldout("References")]
    [SerializeField] TextMeshProUGUI _debugStateText;
    [Foldout("References")]
    [SerializeField] protected Image _crossHair;
    [Foldout("References")]
    [SerializeField] protected Image _crossHairOutline;
    [Foldout("References")]
    [SerializeField] protected GameObject _ui;
    [Foldout("References")]
    [SerializeField]
    protected Transform _cameraTransform;


    PlayerInputMap _inputs;
    protected ArmFSM _fsm;
    #endregion

    #region Stats Decleration
    [Foldout("Stats")]
    [SerializeField] protected float _cooldown;
    protected float _cooldownT;
    #endregion 

    protected virtual void Awake()
    {
        _fsm = GetComponent<ArmFSM>();
        _inputs = new PlayerInputMap();
        _inputs.Actions.Skill.started += _ => PressSong();
        _inputs.Actions.Cancel.started += _ => PressCancelSong();
        _inputs.Actions.Skill.canceled += _ => ReleaseSong();
    }

    void Start()
    {

    }

    void Update()
    {
        //State update
        if (_fsm.currentState != null)
        {
            _fsm.currentState.StateUpdate();
        }

        Debugging();
    }

    #region Input presses
    protected virtual void PressCancelSong()
    {
        bool canCancel = _fsm.currentState.Name == ArmStateList.PREVIS;
        if (canCancel)
            CancelSong();
    }

    protected virtual void PressSong()
    {
        bool canSong = _fsm.currentState.Name == ArmStateList.IDLE;
        if (canSong)
        {
            _fsm.ChangeState(ArmStateList.PREVIS);
        }
    }

    protected virtual void ReleaseSong()
    {
        bool canRelease = _fsm.currentState.Name == ArmStateList.PREVIS;
        if (canRelease)
        {
            _fsm.ChangeState(ArmStateList.ACTIVE);
        }
    }
    #endregion

    protected virtual void CancelSong()
    {
        _fsm.ChangeState(ArmStateList.IDLE);
    }

    public virtual void StartPrevis()
    {

    }

    public virtual void UpdatePrevis()
    {

    }

    public virtual void StopPrevis()
    {

    }

    public virtual void StartActive()
    {
        _fsm.ChangeState(ArmStateList.RECOVERY);
        _crossHairOutline.enabled = false;
    }

    public virtual void UpdateActive()
    {

    }

    public virtual void StopActive()
    {

    }

    public virtual void StartRecovery()
    {
        _cooldownT = _cooldown;
    }

    public void UpdateCooldown()
    {
        _cooldownT -= Time.deltaTime;
        _crossHair.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(_cooldown, 0, _cooldownT));
        if (_cooldownT <= 0f)
        {
            _fsm.ChangeState(ArmStateList.IDLE);
        }
    }

    public virtual void StartIdle()
    {
        Refilled();
    }

    public virtual void CheckLookedAt()
    {

    }

    protected void Refilled()
    {
        _crossHairOutline.enabled = true;
        PlaceHolderSoundManager.Instance.PlayArmFilled();
    }

    #region Debugging
    void Debugging()
    {
#if UNITY_EDITOR
        DebugDisplayArmState();
#endif
    }

    public void DebugDisplayArmState()
    {
        if (_debugStateText && _fsm.currentState != null)
            _debugStateText.text = ("Arm state: " + _fsm.currentState.Name);
    }
    #endregion

    #region Enable Disable Inputs
    void OnEnable()
    {
        _ui.SetActive(true);
        _inputs.Enable();
    }

    void OnDisable()
    {
        _ui.SetActive(false);
        _inputs.Disable();
    }
    #endregion
}
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
    [SerializeField] protected Transform _cameraTransform;

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
        if (_inputs.Actions.Skill.IsPressed())
            PressSong();
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

    #region Swaying
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
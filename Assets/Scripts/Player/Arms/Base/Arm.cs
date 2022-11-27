using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Arm : MonoBehaviour
{
    #region References Decleration
    [Header("References")]
    [SerializeField] TextMeshProUGUI _debugStateText;
    PlayerInputMap _inputs;
    ArmFSM _fsm;
    #endregion

    #region Stats Decleration
    //[Header("Stats")]
    #endregion 

    void Awake()
    {
        _fsm = GetComponent<ArmFSM>();
        _inputs = new PlayerInputMap();
    }

    void Start()
    {
        
    }

    void Update()
    {
        //State update
        if (_fsm.currentState != null)
            _fsm.currentState.StateUpdate();

        Debugging();
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
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
    #endregion
}
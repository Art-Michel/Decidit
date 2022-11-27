using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Arm : MonoBehaviour
{
    #region References Decleration
    [Header("References")]
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
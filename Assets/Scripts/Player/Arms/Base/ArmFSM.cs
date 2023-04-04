using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmFSM : MonoBehaviour
{
    private Dictionary<string, ArmState> states;
    Arm _arm;

    public ArmState CurrentState { get; private set; }
    public ArmState PreviousState { get; private set; }

    void Awake()
    {
        _arm = GetComponent<Arm>();
        states = new Dictionary<string, ArmState>();
    }

    protected virtual void Start()
    {
        AddState(new ArmStateIdle());
        AddState(new ArmStateActive());
        AddState(new ArmStatePrevis());
        AddState(new ArmStateRecovery());

        ChangeState(ArmStateList.IDLE);
    }

    public void AddState(ArmState state)
    {
        state._fsm = this;
        state._arm = this._arm;
        states[state.Name] = state;
    }

    public void ChangeState(string nextStateName)
    {
        ArmState nextState = null;
        states.TryGetValue(nextStateName, out nextState);

        if (nextState == null)
        {
            Debug.LogError(nextStateName + " doesn't exist");
            return;
        }

        if (nextState == CurrentState)
        {
            Debug.LogWarning("Tried entering " + nextStateName + " state which you were already in");
            return;
        }

        if (CurrentState != null)
            CurrentState.Exit();

        PreviousState = CurrentState;
        CurrentState = nextState;

        CurrentState.Begin();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverFSM : MonoBehaviour
{
    protected Dictionary<string, RevolverState> _states;
    protected Revolver _revolver;

    public RevolverState CurrentState { get; private set; }
    public RevolverState PreviousState { get; private set; }

    protected virtual void Awake()
    {
        _revolver = GetComponent<BaseRevolver>();
        _states = new Dictionary<string, RevolverState>();
    }

    protected virtual void Start()
    {
        AddState(new RevolverStateIdle());
        AddState(new RevolverStateReloading());
        AddState(new RevolverStateShooting());

        ChangeState(RevolverStateList.IDLE);
    }

    public void AddState(RevolverState state)
    {
        state._fsm = this;
        state._revolver = this._revolver;
        _states[state.Name] = state;
    }

    public void ChangeState(string nextStateName)
    {
        RevolverState nextState = null;
        _states.TryGetValue(nextStateName, out nextState);

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

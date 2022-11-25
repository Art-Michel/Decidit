using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverFSM : MonoBehaviour
{
    private Dictionary<string, RevolverState> states;
    Revolver _revolver;

    public RevolverState currentState { get; private set; }
    public RevolverState previousState { get; private set; }

    void Awake()
    {
        _revolver = GetComponent<Revolver>();
        states = new Dictionary<string, RevolverState>();
    }

    void Start()
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
        states[state.Name] = state;
    }

    public void ChangeState(string nextStateName)
    {
        RevolverState nextState = null;
        states.TryGetValue(nextStateName, out nextState);

        if (nextState == null)
        {
            Debug.LogError(nextStateName + " doesn't exist");
            return;
        }

        if (nextState == currentState)
        {
            Debug.LogWarning("Tried entering " + nextStateName + " state which you were already in");
            return;
        }

        if (currentState != null)
            currentState.Exit();

        previousState = currentState;
        currentState = nextState;
        currentState.Begin();
    }
}

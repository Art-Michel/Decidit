using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    private Dictionary<string, PlayerState> states;
    Player _player;

    public PlayerState currentState { get; private set; }
    public PlayerState previousState { get; private set; }

    void Awake()
    {
        _player = GetComponent<Player>();
        states = new Dictionary<string, PlayerState>();
    }

    void Start()
    {
        AddState(new PlayerStateGrounded());
        AddState(new PlayerStateJumping());
        AddState(new PlayerStateAirborne());
        AddState(new PlayerStateSliding());
        AddState(new PlayerStateWallriding());
        AddState(new PlayerStateWalljumping());

        ChangeState(PlayerStatesList.GROUNDED);
    }

    public void AddState(PlayerState state)
    {
        state._fsm = this;
        state._player = this._player;
        states[state.Name] = state;
    }

    public void ChangeState(string nextStateName)
    {
        PlayerState nextState = null;
        states.TryGetValue(nextStateName, out nextState);

        if (nextState == null)
        {
            Debug.LogError(nextStateName + " doesn't exist");
            return;
        }

        if (nextState == currentState)
        {
            Debug.LogWarning("Tried entering "+ nextStateName + " state which you were already in");
            return;
        }

        if (currentState != null)
            currentState.Exit();

        previousState = currentState;
        currentState = nextState;
        currentState.Begin();

        Debug.Log(currentState.Name + " started");
    }

    private void Update()
    {
        if (currentState != null)
            currentState.StateUpdate();
    }
}

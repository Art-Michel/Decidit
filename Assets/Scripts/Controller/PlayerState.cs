using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public PlayerFSM _fsm = null;
    public Player _player = null;

    public string Name { get; private set; }

    public PlayerState(string name)
    {
        this.Name = name;
    }

    public virtual void Begin()
    {

    }

    public virtual void StateUpdate()
    {

    }

    public virtual void Exit()
    {
        
    }
}

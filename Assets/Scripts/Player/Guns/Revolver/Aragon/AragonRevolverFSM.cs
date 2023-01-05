using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolverFSM : RevolverFSM
{
    protected override void Awake()
    {
        _revolver = GetComponent<AragonRevolver>();
        _states = new Dictionary<string, RevolverState>();
    }

    protected override void Start()
    {
        AddState(new AragonRevolverStateIdle());
        AddState(new AragonRevolverStateReloading());
        AddState(new AragonRevolverStateShooting());

        ChangeState(RevolverStateList.IDLE);
    }
}

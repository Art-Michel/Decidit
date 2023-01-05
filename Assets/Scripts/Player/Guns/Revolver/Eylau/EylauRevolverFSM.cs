using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolverFSM : RevolverFSM
{

    protected override void Awake()
    {
        _revolver = GetComponent<EylauRevolver>();
        _states = new Dictionary<string, RevolverState>();
    }

    protected override void Start()
    {
        AddState(new EylauRevolverStateIdle());
        AddState(new EylauRevolverStateReloading());
        AddState(new EylauRevolverStateShooting());

        ChangeState(RevolverStateList.IDLE);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseRevolverFSM : RevolverFSM
{
    protected override void Awake()
    {
        _revolver = GetComponent<MuseRevolver>();
        _states = new Dictionary<string, RevolverState>();
    }

    protected override void Start()
    {
        AddState(new MuseRevolverStateIdle());
        AddState(new MuseRevolverStateReloading());
        AddState(new MuseRevolverStateShooting());

        ChangeState(RevolverStateList.IDLE);
    }
}

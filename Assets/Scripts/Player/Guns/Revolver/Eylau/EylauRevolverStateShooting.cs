using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolverStateShooting : RevolverState
{
    public EylauRevolverStateShooting() : base(RevolverStateList.SHOOTING)
    {

    }

    public override void Begin()
    {
        _revolver.Shoot();
        _revolver.LowerAmmoCount();
        _revolver.StartRecoil();
        //Shooting animation
    }

    public override void StateUpdate()
    {
        _revolver.Recoiling();
    }

    public override void Exit()
    {

    }

}

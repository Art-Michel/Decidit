using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauArm : Arm
{
    public override void StartIdle()
    {
        _crossHairOutline.enabled = true;
    }

    public override void StartActive()
    {
        _crossHairOutline.enabled = false;
    }
}

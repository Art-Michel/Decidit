using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolver : Revolver
{
    public override void Shoot()
    {
        PlaceHolderSoundManager.Instance.PlayEylauShot();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolver : Revolver
{
    public override void Shoot()
    {
        PlaceHolderSoundManager.Instance.PlayAragonShot();
    }
}

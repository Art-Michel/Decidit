using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseRevolver : Revolver
{
    public override void Shoot()
    {
        PlaceHolderSoundManager.Instance.PlayMuseShot();
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
    }
}

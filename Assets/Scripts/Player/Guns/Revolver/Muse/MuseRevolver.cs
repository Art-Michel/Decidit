using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MuseRevolver : Revolver
{

    public override void Shoot()
    {
        PlaceHolderSoundManager.Instance.PlayMuseShot();
        _muzzleFlash.Play();
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
    }
}

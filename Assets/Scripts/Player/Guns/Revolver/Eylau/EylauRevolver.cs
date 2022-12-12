using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EylauRevolver : Revolver
{
    [Header("References")]
    [SerializeField] Pooler _projectile1Pooler;

    public override void Shoot()
    {
        PooledObject shot = _projectile1Pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canon.position, (_currentlyAimedAt - _canon.position).normalized);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayEylauShot();
        _muzzleFlash.Play();
    }
}

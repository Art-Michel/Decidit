using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MuseRevolver : Revolver
{
    [Header("References")]
    [SerializeField] Pooler _pooler;

    public override void Shoot()
    {
        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canon.position, (_currentlyAimedAt - _canon.position).normalized, _camera.forward);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayMuseShot();
        _muzzleFlash.Play();
    }
}

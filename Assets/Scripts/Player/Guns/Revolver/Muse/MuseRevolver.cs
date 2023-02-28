using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class MuseRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] Pooler _pooler;

    public override void Shoot()
    {
        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        ////PlaceHolderSoundManager.Instance.PlayMuseShot();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/MuseMalade/Shoot", 5f, gameObject);
        _muzzleFlash.PlayAll();
    }
}

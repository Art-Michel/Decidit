using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] Pooler _pooler;

    public override void Shoot()
    {
        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
        shot.GetComponent<ProjectileOscillator>().Setup(Vector3.up);

        //Optionnal second shot
        PooledObject shot2 = _pooler.Get();
        shot2.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
        shot2.GetComponent<ProjectileOscillator>().Setup(Vector3.down);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        ////PlaceHolderSoundManager.Instance.PlayAragonShot();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/FugueAragon/BaseShoot", 1f, gameObject);
        _muzzleFlash.PlayAll();
    }
}

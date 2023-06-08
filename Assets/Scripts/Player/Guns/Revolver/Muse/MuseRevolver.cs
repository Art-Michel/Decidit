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
        base.Shoot();
        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);

        Player.Instance.StartKickShake(_shootShake, transform.position);
        ////PlaceHolderSoundManager.Instance.PlayMuseShot();
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/MuseMalade/Shoot", 5f, gameObject);
        _muzzleFlash.PlayAll();
        if (!sh1)
        {
            sh1 = true;
            Animator.CrossFade("shoot", 0, 0);
        }
        else
        {
            sh1 = false;
            Animator.CrossFade("shooot", 0, 0);
        }
    }
}

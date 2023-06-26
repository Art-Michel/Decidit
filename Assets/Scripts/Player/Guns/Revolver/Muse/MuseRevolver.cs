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
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/MuseMalade/Shoot", 1f, gameObject);
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

    public override void StartReload()
    {
        SoundManager.Instance.PlaySound("event:/Alexis/SFX/SFX_PLAYER/SFX_PLAYER_Weapons/SFX_PLAYER_Weapons_Muse/SFX_PLAYER_Weapons_Muse_Reload", 1f, gameObject);
        base.StartReload();
    }
}

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonRevolver : Revolver
{
    [SerializeField] private float _adjacentBulletsAngle = 8.0f;
    [Foldout("References")]
    [SerializeField] Pooler _pooler;
    [SerializeField] float _centralDamage = 0.75f;
    [SerializeField] float _sideDamage = 0.5f;

    // [Foldout("BulletsDirection")]
    // [SerializeField] bool _shotsAreCentered;
    // [Foldout("BulletsDirection")]
    // [SerializeField] Vector3[] _shotsDirections;
    // [Foldout("BulletsDirection")]
    // [SerializeField] float[] _offsets;

    public override void Shoot()
    {
        // for (int i = 0; i < _shotsDirections.Length; i++)
        //     SetShot(_shotsDirections[i], _offsets[i]);
        base.Shoot();
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

        Vector3 rightDirection;
        Vector3 leftDirection;

        //Shoot homing bullets
        if (Synergies.Instance.Hospital.Count > 0)
        {
            rightDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * (_adjacentBulletsAngle * 0.5f) + _camera.right).normalized;
            leftDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * (_adjacentBulletsAngle * 0.5f) - _camera.right).normalized;

            PlayerManager.Instance.StartFlash(0.1f, 1);
            SoundManager.Instance.PlaySound("event:/SFX_Controller/UniversalSound", 1f, gameObject);

            FugueMaladeShot shot = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();
            FugueMaladeShot shot2 = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();
            FugueMaladeShot shot3 = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();

            shot.Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward, true);
            shot2.Setup(_canonPosition.position, rightDirection, _camera.forward, false);
            shot3.Setup(_canonPosition.position, leftDirection, _camera.forward, false);
        }

        //Shoot normal Bullets
        else
        {
            rightDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * _adjacentBulletsAngle + _camera.right).normalized;
            leftDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * _adjacentBulletsAngle - _camera.right).normalized;

            FugueProjectile shot = _pooler.Get().GetComponent<FugueProjectile>();
            FugueProjectile shot2 = _pooler.Get().GetComponent<FugueProjectile>();
            FugueProjectile shot3 = _pooler.Get().GetComponent<FugueProjectile>();

            shot.Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward, _centralDamage);
            shot2.Setup(_canonPosition.position, rightDirection, _camera.forward, _sideDamage);
            shot3.Setup(_canonPosition.position, leftDirection, _camera.forward, _sideDamage);
        }

        Player.Instance.StartKickShake(_shootShake, transform.position);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/FugueAragon/BaseShoot", 1f, gameObject);
        _muzzleFlash.PlayAll();
    }

    public override void StartReload()
    {
        SoundManager.Instance.PlaySound("event:/Alexis/SFX/SFX_PLAYER/SFX_PLAYER_Weapons/SFX_PLAYER_Weapons_Aragon/SFX_PLAYER_Weapons_Aragon_Reload", 1f, gameObject);
        base.StartReload();
    }

    // private void SetShot(Vector3 direction, float offsetLength)
    // {
    //     direction = direction.normalized;

    //     PooledObject shot = _pooler.Get();
    //     shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
    //     shot.GetComponent<ProjectileOscillator>().Setup(direction, _shotsAreCentered, offsetLength);
    // }
}

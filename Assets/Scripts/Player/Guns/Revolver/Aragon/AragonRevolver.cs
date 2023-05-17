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

        Vector3 rightDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * _adjacentBulletsAngle + _camera.right).normalized;
        Vector3 leftDirection = ((_currentlyAimedAt - _canonPosition.position).normalized * _adjacentBulletsAngle - _camera.right).normalized;

        if (Synergies.Instance.Hospital.Count > 0)
        {

            FugueMaladeShot shot = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();
            FugueMaladeShot shot2 = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();
            FugueMaladeShot shot3 = Synergies.Instance.FugueMaladeShotsPooler.Get().GetComponent<FugueMaladeShot>();

            shot.Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
            shot2.Setup(_canonPosition.position, rightDirection, _camera.forward);
            shot3.Setup(_canonPosition.position, leftDirection, _camera.forward);
        }
        else
        {
            FugueProjectile shot = _pooler.Get().GetComponent<FugueProjectile>();
            FugueProjectile shot2 = _pooler.Get().GetComponent<FugueProjectile>();
            FugueProjectile shot3 = _pooler.Get().GetComponent<FugueProjectile>();

            shot.Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
            shot2.Setup(_canonPosition.position, rightDirection, _camera.forward);
            shot3.Setup(_canonPosition.position, leftDirection, _camera.forward);
        }

        Player.Instance.StartKickShake(_shootShake, transform.position);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/FugueAragon/BaseShoot", 1f, gameObject);
        _muzzleFlash.PlayAll();
        Animator.CrossFade("shoot", 0.0f, 0);
    }

    // private void SetShot(Vector3 direction, float offsetLength)
    // {
    //     direction = direction.normalized;

    //     PooledObject shot = _pooler.Get();
    //     shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
    //     shot.GetComponent<ProjectileOscillator>().Setup(direction, _shotsAreCentered, offsetLength);
    // }
}

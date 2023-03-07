using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] Pooler _pooler;

    [Foldout("BulletsDirection")]
    [SerializeField] bool _shotsAreCentered;
    [Foldout("BulletsDirection")]
    [SerializeField] Vector3[] _shotsDirections;
    [Foldout("BulletsDirection")]
    [SerializeField] float[] _offsets;

    public override void Shoot()
    {
        for (int i = 0; i < _shotsDirections.Length; i++)
            SetShot(_shotsDirections[i], _offsets[i]);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/FugueAragon/BaseShoot", 1f, gameObject);
        _muzzleFlash.PlayAll();
    }

    private void SetShot(Vector3 direction, float offsetLength)
    {
        direction = direction.normalized;

        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
        shot.GetComponent<ProjectileOscillator>().Setup(direction, _shotsAreCentered, offsetLength);
    }
}

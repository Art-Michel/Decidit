using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class AragonRevolver : Revolver
{
    [Foldout("References")]
    [SerializeField] Pooler _pooler;

    [Foldout("Directions")]
    [SerializeField] bool _shotsAreCentered;
    [Foldout("Directions")]
    [SerializeField] Vector3[] _shotsDirections;

    public override void Shoot()
    {
        foreach (Vector3 direction in _shotsDirections)
            SetShot(direction);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/FugueAragon/BaseShoot", 1f, gameObject);
        _muzzleFlash.PlayAll();
    }

    private void SetShot(Vector3 direction)
    {
        direction = direction.normalized;

        PooledObject shot = _pooler.Get();
        shot.GetComponent<Projectile>().Setup(_canonPosition.position, (_currentlyAimedAt - _canonPosition.position).normalized, _camera.forward);
        shot.GetComponent<ProjectileOscillator>().Setup(direction, _shotsAreCentered);
    }
}

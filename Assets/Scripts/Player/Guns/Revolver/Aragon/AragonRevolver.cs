using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolver : Revolver
{
    [Header("References")]
    [SerializeField] GameObject _projectilePrefab;

    public override void Shoot()
    {
        GameObject Shot = Instantiate(_projectilePrefab, _canon.position, Quaternion.Euler(_currentlyAimedAt - _canon.position));
        Shot.GetComponent<Projectile>().Setup((_currentlyAimedAt - _canon.position).normalized, _camera.forward);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayAragonShot();
    }
}

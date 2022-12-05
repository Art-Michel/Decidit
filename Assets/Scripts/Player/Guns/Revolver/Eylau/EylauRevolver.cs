using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolver : Revolver
{
    [Header("References")]
    [SerializeField] GameObject _unchargedProjectilePrefab;

    public override void Shoot()
    {
        GameObject Shot = Instantiate(_unchargedProjectilePrefab, _canon.position, Quaternion.Euler(_currentlyAimedAt - _canon.position));
        Shot.GetComponent<Projectile>().Setup((_currentlyAimedAt - _canon.position).normalized);

        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayEylauShot();
    }
}

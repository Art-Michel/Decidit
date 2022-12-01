using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AragonRevolver : Revolver
{
    [Header("References")]
    [SerializeField] GameObject _projectilePrefab;

    public override void Shoot()
    {
        Instantiate(_projectilePrefab, _canon.position, _camera.rotation);
        
        Player.Instance.StartShake(_shootShakeIntensity, _shootShakeDuration);
        PlaceHolderSoundManager.Instance.PlayAragonShot();
    }
}

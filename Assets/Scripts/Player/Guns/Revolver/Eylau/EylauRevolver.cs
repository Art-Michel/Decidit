using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauRevolver : Revolver
{
    [Header("References")]
    [SerializeField] GameObject _unchargedProjectilePrefab;

    public override void Shoot()
    {
        Instantiate(_unchargedProjectilePrefab, _canon.position, _camera.rotation);
        PlaceHolderSoundManager.Instance.PlayEylauShot();
    }
}

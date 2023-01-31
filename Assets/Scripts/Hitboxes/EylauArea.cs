using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class EylauArea : MonoBehaviour
{
    [SerializeField] private LayerMask _shouldHit;
    [SerializeField] private LayerMask _shouldEnhance;
    [SerializeField] private LayerMask _shouldBuff;
    [SerializeField] private float _radius;
    private bool _isPlayerInHere = false;
    private bool _wasPlayerInHere = false;

    void Update()
    {
        CheckForEnemies();
        CheckForBullets();
        CheckForPlayer();
    }

    private void CheckForEnemies()
    {
        foreach (Collider collider in Physics.OverlapCapsule(transform.position + Vector3.down * 100, transform.position + Vector3.up * 100, _radius, _shouldHit))
            Debug.Log("j'ai rien codé ici pour l'instant");
    }

    private bool AlreadyHit(Transform target)
    {
        return true;
    }

    private void CheckForBullets()
    {

    }

    private void CheckForPlayer()
    {
        _isPlayerInHere = Physics.OverlapCapsule(transform.position + Vector3.down * 100, transform.position + Vector3.up * 100, _radius, _shouldBuff).Length > 0;
        if (!_isPlayerInHere && _wasPlayerInHere)
        {
            Player.Instance.ResetMovement();
        }
        if (_isPlayerInHere && !_wasPlayerInHere)
        {
            Player.Instance.BuffMovement();
        }
        _wasPlayerInHere = _isPlayerInHere;
    }

    void OnDisable()
    {

    }

}

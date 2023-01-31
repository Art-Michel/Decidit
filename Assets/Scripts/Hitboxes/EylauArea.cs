using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using State.AIBull;
using State.AICAC;
using State.FlyAI;
using State.WallAI;

public class EylauArea : MonoBehaviour
{
    [SerializeField] private LayerMask _shouldEnhance;
    [SerializeField] private LayerMask _shouldBuff;
    [SerializeField] private float _radius;
    private bool _isPlayerInHere = false;
    private bool _wasPlayerInHere = false;

    void Update()
    {
        CheckForBullets();
        CheckForPlayer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {

            if (TryGetComponent<GlobalRefAICAC>(out GlobalRefAICAC cac))
            {

            }
            //globalRefAICAC.isInEylau = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ennemi"))
        {
            //globalRefAICAC.isInEylau = false;
        }
    }

    // private void CheckForEnemies()
    // {
    //     Collider[] colliders = Physics.OverlapCapsule(transform.position + Vector3.down * 100, transform.position + Vector3.up * 100, _radius, _shouldEnhance);

    //     //foreach enemy detected inside
    //     foreach (Collider collider in colliders)
    //     {
    //         if (!_enemiesInTrigger.ContainsKey(collider))
    //         {
    //             Debug.Log("nerfed enemy");
    //             _enemiesInTrigger.Add(collider, collider.GetComponent<Transform>());
    //             //TODO  if (!_enemiesInTrigger[collider].IsNerfed)
    //             //TODO    _enemiesInTrigger[collider].Nerf();
    //         }
    //     }

    //     //foreach enemy in the dictionary
    //     foreach (Collider collider in _enemiesInTrigger.Keys)
    //     {
    //         if (!colliders.Contains<Collider>(collider))
    //         {
    //             _enemiesInTrigger.Remove(collider);
    //             Debug.Log("restored enemy");
    //         }
    //     }
    // }

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
}

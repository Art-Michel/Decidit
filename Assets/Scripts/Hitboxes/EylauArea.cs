using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class EylauArea : MonoBehaviour
{
    [SerializeField] private LayerMask _shouldHit;
    [SerializeField] private float _radius;

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == _shouldHit)
    //     {
    //         if (other.transform.CompareTag("Player"))
    //         {
    //             Debug.Log("players in");
    //         }
    //         if (other.transform.CompareTag("Ennemi"))
    //         {
    //             Debug.Log("enemys in");
    //         }
    //     }
    // }

    void Update()
    {
        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        foreach (Collider collider in Physics.OverlapCapsule(transform.position + Vector3.down * 100, transform.position + Vector3.up * 100, _radius, _shouldHit))
            if (!AlreadyHit(collider.transform.parent))
                Hit(collider.transform);
    }

    private bool AlreadyHit(Transform target)
    {
        return true;
    }

    private void Hit(Transform targetCollider)
    {

    }
}

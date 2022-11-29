using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask _mask;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _speed;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit " + other.gameObject.name);
        if (other.CompareTag("Ennemi"))
            other.transform.GetComponent<Health>().TakeDamage();
    }

    void Update()
    {
        _rb.MovePosition(transform.position + Vector3.forward * Time.deltaTime * _speed);
    }
}
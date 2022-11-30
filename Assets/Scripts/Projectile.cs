using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] float _speed;
    [SerializeField] Vector3 _direction;
    [SerializeField] Vector3 _initialPosition;

    void Start()
    {
        Setup();
    }

    void OnEnable()
    {
        Setup();
    }

    void Setup()
    {
        _initialPosition = transform.position;
    }

    protected override void FixedUpdate()
    {

        transform.position += _direction * _speed;
    }

    protected override void Hit(Collider collider)
    {
        base.Hit(collider);
    }

}
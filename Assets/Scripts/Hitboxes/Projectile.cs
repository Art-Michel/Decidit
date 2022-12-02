using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] protected float _speed = 0.5f;
    protected Vector3 _direction;

    void Start()
    {
        Setup(transform.forward);
    }

    void OnEnable()
    {
        Setup(transform.forward);
    }

    void Setup(Vector3 direction)
    {
        _direction = direction;
    }

    protected override void FixedUpdate()
    {
        float range = (_direction * _speed).magnitude;
        if (Physics.SphereCast(transform.position, _radius, _direction, out RaycastHit hit, range, _shouldCollideWith))
            HitOnce(hit.transform);
        transform.position += _direction * _speed;
    }

    void HitOnce(Transform target)
    {
        Debug.Log(transform.name + " hit " + target.transform.name);
        target.GetComponent<Health>().TakeDamage(_damage);
        DestroyThis();
    }

    void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
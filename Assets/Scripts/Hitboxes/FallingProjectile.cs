using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingProjectile : Projectile
{
    [SerializeField] float _drag = 1f;
    [SerializeField] float _speedReductionFactor = 1f;
    const float _gravity = 9.81f;
    float _currentlyAppliedGravity = 0f;
    float _currentSpeed;

    public override void Setup(Vector3 position, Vector3 direction)
    {
        base.Setup(position, direction);
        _currentlyAppliedGravity = 0f;
        _currentSpeed = _speed;
    }

    public override void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        base.Setup(position, direction, cameraDirection);
        _currentlyAppliedGravity = 0f;
        _currentSpeed = _speed;
    }


    protected override void Move()
    {
        _currentlyAppliedGravity -= _gravity * _drag * Time.deltaTime;
        _currentSpeed -= _speedReductionFactor * Time.deltaTime;

        Vector3 direction = _direction * _speed + Vector3.up * _currentlyAppliedGravity;
        transform.position += direction * Time.deltaTime;
    }
}

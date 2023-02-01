using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingProjectile : Projectile
{
    float mario;
    [SerializeField] float _drag = 1f;
    [SerializeField] float _speedReductionFactor = 1f;
    const float _gravity = 9.81f;
    float _currentlyAppliedGravity = 0f;

    public override void Setup(Vector3 position, Vector3 direction)
    {
        base.Setup(position, direction);
        _currentlyAppliedGravity = 0f;
    }

    public override void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        base.Setup(position, direction, cameraDirection);
        _currentlyAppliedGravity = 0f;
    }

    protected override void Move()
    {
        _currentlyAppliedGravity -= _gravity * _drag * Time.deltaTime;
        _currentSpeed -= _speedReductionFactor * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0f, _speed);

        Vector3 direction = _direction * _currentSpeed + Vector3.up * _currentlyAppliedGravity;
        transform.position += direction * Time.deltaTime;
    }

    protected override void Bounce(RaycastHit hit)
    {
        transform.position = hit.point + hit.normal * (_radius);

        _direction = ((_direction * _currentSpeed) + Vector3.up * (_currentlyAppliedGravity)).normalized;
        _direction = Vector3.Reflect(_direction, hit.normal).normalized;

        _currentSpeed += Mathf.Abs(_currentlyAppliedGravity) * _bounciness;
        _currentSpeed *= _bounciness;
        _currentlyAppliedGravity = 0f;

        _lasterFramePosition = hit.point + hit.normal * (_radius + 0.1f);
        _lastFramePosition = hit.point + hit.normal * (_radius + 0.1f);
    }
}

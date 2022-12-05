using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _lifeSpan = 5f;
    protected Vector3 _direction;
    protected Vector3 _cameraDirection;
    protected Vector3 _lasterFramePosition = Vector3.zero;
    protected Vector3 _lastFramePosition = Vector3.zero;
    protected Vector3 _spaceTraveledLastFrame = Vector3.zero;

    public void Setup(Vector3 direction)
    {
        _direction = direction;
    }

    public void Setup(Vector3 direction, Vector3 cameraDirection)
    {
        _direction = direction;
        _cameraDirection = cameraDirection;
    }

    protected override void Update()
    {
        _lasterFramePosition = _lastFramePosition;
        _lastFramePosition = transform.position;
        transform.position += _direction * _speed * Time.deltaTime;
        _spaceTraveledLastFrame = transform.position - _lastFramePosition;

        CheckForCollision();

        UpdateLifeSpan();

        if (_canMultiHit)
            UpdateBlackList();
    }

    private void UpdateLifeSpan()
    {
        _lifeSpan -= Time.deltaTime;
        if (_lifeSpan <= 0)
        {
            Disappear();
        }
    }

    protected override void CheckForCollision()
    {
        if (_canMultiHit)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(_lasterFramePosition, _radius, _spaceTraveledLastFrame, _spaceTraveledLastFrame.magnitude, _shouldCollideWith))
                if (!AlreadyHit(hit.transform))
                {
                    Hit(hit.transform);
                    _direction = _cameraDirection;
                }
        }
        else if (Physics.SphereCast(_lasterFramePosition, _radius, _spaceTraveledLastFrame, out RaycastHit hit, _spaceTraveledLastFrame.magnitude, _shouldCollideWith))
        {
            Hit(hit.transform);
            Disappear();
        }
    }

    protected void Disappear()
    {
        Destroy(this.gameObject);
    }
}
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PooledObject))]
public class Projectile : Hitbox
{
    [Header("Projectile values")]
    [SerializeField] private PooledObject _pooledObject;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] protected float _speed = 100f;
    [SerializeField] private float _lifeSpan = 5f;
    [SerializeField] private float _trailDelay = 0.025f;
    private float _lifeT;
    private float _trailDelayT;
    protected Vector3 _direction;
    protected Vector3 _cameraDirection;
    protected Vector3 _lasterFramePosition;
    protected Vector3 _lastFramePosition;
    protected Vector3 _spaceTraveledLastFrame;

    public virtual void Setup(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        _direction = direction;
        _lifeT = _lifeSpan;
        _trailDelayT = _trailDelay; //Delay before spawning the trail
        _trailRenderer.enabled = false;
        _mesh.SetActive(false);
        _lasterFramePosition = position;
        _lastFramePosition = position;
        _spaceTraveledLastFrame = position;
    }

    public virtual void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        Setup(position, direction);
        _cameraDirection = cameraDirection;
    }

    protected override void Awake()
    {
        base.Awake();
        _mesh.SetActive(false);
    }

    protected override void Update()
    {
        _lasterFramePosition = _lastFramePosition;
        _lastFramePosition = transform.position;

        Move();
        _spaceTraveledLastFrame = transform.position - _lastFramePosition;

        CheckForCollision();

        UpdateLifeSpan();

        if (_canMultiHit)
            UpdateBlackList();
    }

    protected virtual void Move()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void UpdateLifeSpan()
    {
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            Disappear();
        }
        if (_trailDelayT > 0)
        {
            _trailDelayT -= Time.deltaTime;
            if (_trailDelayT <= 0)
            {
                _trailRenderer.enabled = true;
                _mesh.SetActive(true);
            }
        }
    }

    protected override void CheckForCollision()
    {
        if (_canMultiHit)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(_lasterFramePosition, _radius, _spaceTraveledLastFrame, _spaceTraveledLastFrame.magnitude, _shouldCollideWith))
                if (!AlreadyHit(hit.transform.parent))
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
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        _pooledObject.Pooler.Return(_pooledObject);
    }
}
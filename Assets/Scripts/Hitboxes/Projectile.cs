using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PooledObject))]
public class Projectile : Hitbox
{
    [Header("Projectile values")]
    [SerializeField] private PooledObject _pooledObject;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private MonoBehaviour _trailMaterial;
    [SerializeField] private bool _bounces;
    [SerializeField][ShowIf("_bounces")][Range(0f, 1f)] protected float _bounciness;
    [SerializeField] private bool _explodesOnHit;
    [SerializeField][ShowIf("_explodesOnHit")] private GameObject _explosion;
    [SerializeField] protected float _speed = 100f;
    [SerializeField] protected float _currentSpeed;
    [SerializeField] private float _lifeSpan = 5f;
    [SerializeField] private float _trailDelay = 0.025f;
    private float _lifeT;
    private float _trailDelayT;
    protected Vector3 _direction;
    protected Vector3 _cameraDirection;
    protected Vector3 _lasterFramePosition;
    protected Vector3 _lastFramePosition;
    protected Vector3 _spaceTraveledLast2Frames;

    public virtual void Setup(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        _direction = direction;
        _lifeT = _lifeSpan;
        _trailDelayT = _trailDelay; //Delay before spawning the trail
        if (_trailRenderer) _trailRenderer.enabled = false;
        if (_trailMaterial) _trailMaterial.enabled = false;
        _mesh.SetActive(false);
        _lasterFramePosition = position - _direction * _radius;
        _lastFramePosition = position - _direction * _radius;
        _spaceTraveledLast2Frames = Vector3.zero;
        _currentSpeed = _speed;
        this.enabled = true;
    }

    public virtual void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        Setup(position, direction);
        _cameraDirection = cameraDirection;
        _currentSpeed = _speed;
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
        _spaceTraveledLast2Frames = transform.position - _lasterFramePosition;

        CheckForCollision();

        UpdateLifeSpan();

        if (_canMultiHit)
            UpdateBlackList();
    }

    protected virtual void Move()
    {
        transform.position += _direction * _currentSpeed * Time.deltaTime;
    }

    private void UpdateLifeSpan()
    {
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            if (_explodesOnHit)
                Explode(Vector3.zero);
            else
                Disappear();
        }
        if (_trailDelayT > 0)
        {
            _trailDelayT -= Time.deltaTime;
            if (_trailDelayT <= 0)
            {
                // spawn trail after a bit
                if (_trailRenderer) _trailRenderer.enabled = true;
                if (_trailMaterial) _trailMaterial.enabled = true;
                _mesh.SetActive(true);
            }
        }
    }

    protected override void CheckForCollision()
    {
        //if the bullet can multihit, it is piercing, no questions, just hit what you can
        if (_canMultiHit)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(_lasterFramePosition, _radius, _spaceTraveledLast2Frames.normalized, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
                if (!AlreadyHit(hit.transform.parent))
                {
                    Hit(hit.transform);
                    _direction = _cameraDirection;
                }
        }

        //if the bullet should disappear on hit, we gotta first check whether it is a bouncing ball
        else if (Physics.SphereCast(_lasterFramePosition, _radius, _spaceTraveledLast2Frames.normalized, out RaycastHit hit, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
        {
            if (_bounces)
            {
                //will not bounce if hit a direct enemy
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHurtbox"))
                {
                    Hit(hit.transform);
                    //+ explostion if projectile should spawn an explosion.
                    if (_explodesOnHit)
                        Explode(hit.normal);
                    else
                        Disappear();
                }
                else
                    Bounce(hit);
            }

            //if does not bounce nor multihit, just hit and disappear
            else
            {
                Hit(hit.transform);
                //+ explostion if projectile should spawn an explosion.
                if (_explodesOnHit)
                    Explode(hit.normal);
                else
                    Disappear();
            }
        }
    }

    protected virtual void Bounce(RaycastHit hit)
    {
        transform.position = hit.point + hit.normal * (_radius + 0.1f);
        _currentSpeed *= _bounciness;
        _direction = Vector3.Reflect(_direction, hit.normal).normalized;
        _lasterFramePosition = hit.point + hit.normal * (_radius + 0.1f);
        _lastFramePosition = hit.point + hit.normal * (_radius + 0.1f);
    }

    private void Explode(Vector3 normal)
    {
        if (normal != Vector3.zero)
            _explosion.transform.up = normal;

        _explosion.SetActive(true);
        _mesh.SetActive(false);
        this.enabled = false;
    }

    public void Disappear()
    {
        if (_trailRenderer)
        {
            _trailRenderer.Clear();
            _trailRenderer.enabled = false;
        }
        if (_trailMaterial)
        {
            _trailMaterial.enabled = false;
        }
        _pooledObject.Pooler.Return(_pooledObject);
    }
}
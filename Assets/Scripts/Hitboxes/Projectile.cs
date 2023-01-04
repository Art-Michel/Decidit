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
    [SerializeField] private bool _bounces;
    [SerializeField] private bool _explodesOnHit;
    [SerializeField][ShowIf("_explodesOnHit")] private GameObject _explosion;
    [SerializeField] protected float _speed = 100f;
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
        _trailRenderer.enabled = false;
        _mesh.SetActive(false);
        _lasterFramePosition = position;
        _lastFramePosition = position;
        _spaceTraveledLast2Frames = Vector3.zero;
        this.enabled = true;
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
        _spaceTraveledLast2Frames = transform.position - _lasterFramePosition;

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
        //if the bullet can multihit, it is piercing, no questions, just hit what you can
        if (_canMultiHit)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(_lasterFramePosition, _radius, _spaceTraveledLast2Frames, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
                if (!AlreadyHit(hit.transform.parent))
                {
                    Hit(hit.transform);
                    _direction = _cameraDirection;
                }
        }
        //if the bullet should disappear on hit, we gotta first check whether it is a bouncing ball
        else if (Physics.SphereCast(_lasterFramePosition, _radius, _spaceTraveledLast2Frames, out RaycastHit hit, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
        {
            if (_bounces)
            {
                //will not bounce if hit a direct enemy
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHurtbox"))
                {
                    Hit(hit.transform);
                    //+ explostion if projectile should spawn an explosion.
                    if (_explodesOnHit)
                    {
                        _explosion.SetActive(true);
                        _mesh.SetActive(false);
                        this.enabled = false;
                    }
                }
                else
                {
                    //bounce formula;
                }
            }

            //if does not bounce nor multihit, just hit and disappear
            else
            {
                Hit(hit.transform);
                //+ explostion if projectile should spawn an explosion.
                if (_explodesOnHit)
                {
                    _explosion.SetActive(true);
                    _mesh.SetActive(false);
                    this.enabled = false;
                }
                Disappear();
            }
        }
    }

    public void Disappear()
    {
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        _pooledObject.Pooler.Return(_pooledObject);
    }
}
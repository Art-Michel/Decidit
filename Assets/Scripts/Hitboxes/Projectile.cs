using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(PooledObject))]
public class Projectile : Hitbox
{

    [Foldout("References")]
    [SerializeField] private PooledObject _pooledObject;
    [Foldout("References")]
    [SerializeField] private GameObject _mesh;
    [Foldout("References")]
    [SerializeField] private TrailRenderer[] _trailRenderer;
    [Foldout("References")]
    [SerializeField] private MonoBehaviour _trailMaterial;
    [Foldout("References")]
    [SerializeField] private Pooler _impactVfxPooler;
    [Foldout("References")]
    [SerializeField] private Pooler _fleshSplashVfxPooler;

    [Foldout("Properties")]
    [SerializeField] protected bool _shouldLeaveImpact;
    [Foldout("Properties")]
    [SerializeField] private bool _bounces;
    [Foldout("Properties")]
    [SerializeField][ShowIf("_bounces")][Range(0f, 1f)] protected float _bounciness;
    [Foldout("Properties")]
    [SerializeField] private bool _explodesOnHit;
    [Foldout("References")]
    [SerializeField][ShowIf("_explodesOnHit")] private GameObject _explosion;

    [Foldout("Stats")]
    [SerializeField] protected float _speed = 100f;
    [Foldout("Stats")]
    [SerializeField] protected float _currentSpeed;
    [Foldout("Stats")]
    [SerializeField] private float _lifeSpan = 5f;
    [Foldout("Stats")]
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
        transform.rotation = Camera.main.transform.rotation;

        _direction = direction;
        _lifeT = _lifeSpan;
        _trailDelayT = _trailDelay; //Delay before spawning the trail
        if (_trailRenderer.Length > 0)
            foreach (TrailRenderer trail in _trailRenderer)
                trail.enabled = false;
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
        if (_trailDelayT >= 0)
        {
            _trailDelayT -= Time.deltaTime;
            if (_trailDelayT < 0)
            {
                // spawn trail after a bit
                if (_trailRenderer.Length > 0)
                    foreach (TrailRenderer trail in _trailRenderer)
                        trail.enabled = true;
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
                    if (_shouldLeaveImpact)
                        LeaveImpact(hit, false);

                    //Reset direction to camera direction in order to cancel the fact we initially sent the
                    //projectile slightly angled to compensate the gun's offset
                    _direction = _cameraDirection;
                }
            //TODO Art overlapsphere sur place pour fix le multihit
            // foreach (Collider collider in Physics.OverlapSphere(transform.position, _radius, _shouldCollideWith))
            // {

            //     if (!AlreadyHit(hit.transform.parent))
            //     {
            //         Hit(hit.transform);
            //         if (_shouldLeaveImpact)
            //             LeaveImpact(collider, false);

            //         //Reset direction to camera direction in order to cancel the fact we initially sent the
            //         //projectile slightly angled to compensate the gun's offset
            //         _direction = _cameraDirection;
            //     }
            // }

            //second raycast backwards to leave impact after exiting a surface
            if (_shouldLeaveImpact)
                foreach (RaycastHit hit in Physics.RaycastAll(transform.position, -_spaceTraveledLast2Frames.normalized, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
                    LeaveImpact(hit, true);

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
                {
                    if (_shouldLeaveImpact) LeaveImpact(hit, false);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        Bounce(hit);
                }
            }

            //if does not bounce nor multihit, just hit and disappear
            else
            {
                Hit(hit.transform);
                if (_shouldLeaveImpact)
                    LeaveImpact(hit, false);

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

    private void LeaveImpact(RaycastHit hit, bool fromBehind)
    {
        //wall or ground
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GameObject impactVfx = _impactVfxPooler.Get().gameObject;
            impactVfx.transform.position = hit.point + hit.normal * 0.05f;
            if (fromBehind)
                impactVfx.transform.forward = -_direction;
            else
                impactVfx.transform.forward = _direction;
        }

        //flesh
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Flesh"))
        {
            GameObject splashVfx = _fleshSplashVfxPooler.Get().gameObject;
            splashVfx.transform.position = hit.point + hit.normal * 0.05f;

            if (fromBehind) // upside down for some reason
                splashVfx.transform.forward = _direction;
            else
                splashVfx.transform.forward = -_direction;
        }
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
        if (_trailRenderer.Length > 0)
            foreach (TrailRenderer trail in _trailRenderer)
            {

                trail.Clear();
                trail.enabled = false;
            }
        if (_trailMaterial)
        {
            _trailMaterial.enabled = false;
        }
        _pooledObject.Pooler.Return(_pooledObject);
    }
}
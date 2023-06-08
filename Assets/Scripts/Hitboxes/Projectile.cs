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
    [SerializeField] private TrailRenderer[] _trailRenderers;
    [Foldout("References")]
    [SerializeField] private MonoBehaviour _trailMaterial;
    [Foldout("References")]
    [SerializeField] private GameObject _trailsVfx;


    [Foldout("Properties")]
    [SerializeField] protected bool _shouldLeaveImpact;
    [Foldout("Properties")]
    [SerializeField] private bool _bounces;
    [Foldout("Properties")]
    [SerializeField][ShowIf("_bounces")][Range(0f, 1f)] protected float _bounciness;
    [Foldout("Properties")]
    [SerializeField][ShowIf("_bounces")] protected string _bounceSoundPath;
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
    [Foldout("Stats")]
    [SerializeField] private float _impactScale = 1.0f;
    private float _lifeT;
    private float _trailDelayT;
    public Vector3 Direction { get; protected set; }
    protected Vector3 _cameraDirection;
    protected Vector3 _lasterFramePosition;
    protected Vector3 _lastFramePosition;
    protected Vector3 _spaceTraveledLast2Frames;
    protected bool _isDisappearing;
    [SerializeField] private float _disappearanceT;

    public virtual void Setup(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.rotation = Camera.main.transform.rotation;
        Direction = direction;

        //trails
        _lifeT = _lifeSpan;
        _trailDelayT = _trailDelay; //Delay before spawning the trail
        if (_trailRenderers.Length > 0)
            foreach (TrailRenderer trail in _trailRenderers)
            {
                trail.emitting = true;
                trail.enabled = true;
                trail.Clear();
            }
        if (_trailMaterial) _trailMaterial.enabled = false;
        if (_trailsVfx)
            _trailsVfx.SetActive(false);
        _mesh.SetActive(false);

        _lasterFramePosition = position - direction * _radius * 2;
        _lastFramePosition = position - direction * _radius;
        _spaceTraveledLast2Frames = position - _lasterFramePosition;
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
        if (_isDisappearing)
            UpdateDisappearance();

        else
        {
            CheckForCollision();
            _lasterFramePosition = _lastFramePosition;
            _lastFramePosition = transform.position;

            Move();
            _spaceTraveledLast2Frames = transform.position - _lasterFramePosition;


            UpdateLifeSpan();

            if (_canMultiHit)
                UpdateBlackList();
        }
    }

    protected virtual void Move()
    {
        transform.position += Direction * _currentSpeed * Time.deltaTime;
    }

    private void UpdateLifeSpan()
    {
        _lifeT -= Time.deltaTime;
        if (_lifeT <= 0)
        {
            if (_explodesOnHit)
                Explode(Vector3.zero);
            else
                StartDisappearing();
        }

        if (_trailDelayT >= 0)
        {
            _trailDelayT -= Time.deltaTime;
            if (_trailDelayT < 0)
            {
                // spawn trail after a bit
                if (_trailRenderers.Length > 0)
                    foreach (TrailRenderer trail in _trailRenderers)
                    {
                        // trail.enabled = true;
                        // trail.Clear();
                        // trail.emitting = true;
                    }
                if (_trailMaterial)
                    _trailMaterial.enabled = true;
                _mesh.SetActive(true);
                if (_trailsVfx)
                    _trailsVfx.SetActive(true);
            }
        }
    }

    protected override void CheckForCollision()
    {
        //if the bullet can multihit, it is piercing, no questions, just hit what you can
        if (_canMultiHit)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(_lasterFramePosition, _radius, _spaceTraveledLast2Frames.normalized, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
                if (hit.transform.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
                {
                    if (!AlreadyHit(hurtbox.HealthComponent.transform))
                    {
                        Hit(hurtbox.transform);
                        if (_shouldLeaveImpact)
                            LeaveImpact(hit.transform, hit.point, false);

                        //Reset direction to camera direction in order to cancel the fact we initially sent the
                        //projectile slightly angled to compensate the gun's offset
                        Direction = _cameraDirection;
                    }
                }
                else if (!AlreadyHit(hit.transform))
                {
                    Hit(hit.transform);
                    if (_shouldLeaveImpact)
                        LeaveImpact(hit.transform, hit.point, false);

                    Direction = _cameraDirection;
                }

            foreach (Collider collider in Physics.OverlapSphere(transform.position, _radius, _shouldCollideWith))
            {
                if (collider.transform.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
                {
                    if (!AlreadyHit(hurtbox.HealthComponent.transform))
                    {
                        Hit(hurtbox.transform);
                        if (_shouldLeaveImpact)
                            LeaveImpact(collider.transform, transform.position, false);

                        Direction = _cameraDirection;
                    }
                }
                else if (!AlreadyHit(collider.transform))
                {
                    Hit(collider.transform);
                    if (_shouldLeaveImpact)
                        LeaveImpact(collider.transform, transform.position, false);

                    Direction = _cameraDirection;
                }
            }

            //second raycast backwards to leave impact after exiting a surface
            if (_shouldLeaveImpact)
                foreach (RaycastHit hit in Physics.RaycastAll(transform.position, -_spaceTraveledLast2Frames.normalized, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
                    LeaveImpact(hit.transform, hit.point, true);

        }

        //if the bullet should disappear on hit, we gotta first check whether it is a bouncing ball
        else if (Physics.SphereCast(_lasterFramePosition, _radius, _spaceTraveledLast2Frames.normalized, out RaycastHit hit, _spaceTraveledLast2Frames.magnitude, _shouldCollideWith))
        {
            if (_bounces)
            {
                //will not bounce if hit a direct enemy
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHurtbox"))
                {
                    if (hit.transform.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
                        Hit(hurtbox.transform);
                    else
                        Hit(hit.transform);
                    //+ explostion if projectile should spawn an explosion.
                    if (_explodesOnHit)
                        Explode(hit.normal);
                    else
                        StartDisappearing();
                }
                else
                {
                    if (_shouldLeaveImpact) LeaveImpact(hit.transform, hit.point, false, hit.normal);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        Bounce(hit);
                }
            }
            //if does not bounce nor multihit, just hit and disappear
            else
            {
                if (hit.transform.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
                    Hit(hurtbox.transform);
                else
                    Hit(hit.transform);

                if (_shouldLeaveImpact)
                    LeaveImpact(hit.transform, hit.point, false, hit.normal);
                //+ explostion if projectile should spawn an explosion.
                if (_explodesOnHit)
                    Explode(hit.normal);
                else
                    StartDisappearing();
            }
        }
    }

    protected virtual void Bounce(RaycastHit hit)
    {
        SoundManager.Instance.PlaySound(_bounceSoundPath, 1, gameObject);
        transform.position = hit.point + hit.normal * (_radius + 0.1f);
        _currentSpeed *= _bounciness;
        Direction = Vector3.Reflect(Direction, hit.normal).normalized;
        _lasterFramePosition = hit.point + hit.normal * (_radius + 0.1f);
        _lastFramePosition = hit.point + hit.normal * (_radius + 0.1f);
    }

    private void LeaveImpact(Transform obj, Vector3 point, bool fromBehind)
    {
        //wall or ground
        if (obj.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PooledObject impactVfx = _impactVfxPooler.Get();
            //Hard limit for impacts so we don't get 10000 vfx and sfx when going through many walls
            if (impactVfx == null)
                return;

            impactVfx.transform.position = point - Direction * 0.01f;
            impactVfx.transform.localScale = Vector3.one * _impactScale;

            if (Direction != Vector3.zero)
            {
                if (fromBehind)
                    impactVfx.transform.forward = -Direction;
                else
                {
                    SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactObject", 1f, gameObject);
                    impactVfx.transform.forward = Direction;
                }
            }
        }

        //flesh
        else if (obj.gameObject.layer == LayerMask.NameToLayer("Flesh"))
        {
            PooledObject splashVfx = _fleshSplashVfxPooler.Get();
            //Hard limit for impacts so we don't get 10000 vfx and sfx when going through many walls
            if (splashVfx == null)
                return;

            splashVfx.transform.position = point - Direction * 0.05f;
            splashVfx.transform.localScale = Vector3.one * _impactScale;

            if (Direction != Vector3.zero)
            {
                if (fromBehind) // upside down for some reason
                    splashVfx.transform.forward = Direction;
                else
                {
                    SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactFlesh", 1f, gameObject);
                    splashVfx.transform.forward = -Direction;
                }
            }
        }
    }

    private void LeaveImpact(Transform obj, Vector3 point, bool fromBehind, Vector3 normal)
    {
        //wall or ground
        if (obj.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PooledObject impactVfx = _impactVfxPooler.Get();
            //Hard limit for impacts so we don't get 10000 vfx and sfx when going through many walls
            if (impactVfx == null)
                return;

            impactVfx.transform.position = point + normal * 0.05f;
            impactVfx.transform.localScale = Vector3.one * _impactScale;

            if (Direction != Vector3.zero)
            {
                if (fromBehind)
                    impactVfx.transform.forward = normal;
                else
                {
                    SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactObject", 1f, gameObject);
                    impactVfx.transform.forward = -normal;
                }
            }
        }

        //flesh
        else if (obj.gameObject.layer == LayerMask.NameToLayer("Flesh"))
        {
            PooledObject splashVfx = _fleshSplashVfxPooler.Get();
            //Hard limit for impacts so we don't get 10000 vfx and sfx when going through many walls
            if (splashVfx == null)
                return;

            splashVfx.transform.position = point - Direction * 0.05f;
            splashVfx.transform.localScale = Vector3.one * _impactScale;

            if (Direction != Vector3.zero)
            {
                if (fromBehind) // upside down for some reason
                    splashVfx.transform.forward = Direction;
                else
                {
                    SoundManager.Instance.PlaySound("event:/SFX_Controller/Shoots/BaseShoot/BaseShootImpactFlesh", 1f, gameObject);
                    splashVfx.transform.forward = -Direction;
                }
            }
        }
    }

    public void Explode(Vector3 normal)
    {
        if (normal != Vector3.zero)
            _explosion.transform.up = normal;

        _explosion.SetActive(true);
        _mesh.SetActive(false);
        this.enabled = false;
    }

    public void StartDisappearing()
    {
        _isDisappearing = true;
        if (_trailRenderers.Length > 0)
        {
            foreach (TrailRenderer trail in _trailRenderers)
            {
                trail.emitting = false;
            }
            _disappearanceT = 1;
        }

        if (_trailMaterial)
        {
            _trailMaterial.enabled = false;
        }

        if (_trailsVfx != null)
            _trailsVfx.SetActive(false);
    }

    private void UpdateDisappearance()
    {
        _disappearanceT -= Time.deltaTime;
        if (_disappearanceT <= 0.0f)
            Disappear();
    }

    public void Disappear()
    {
        _isDisappearing = false;
        foreach (TrailRenderer trail in _trailRenderers)
        {
            trail.enabled = false;
        }
        _pooledObject.Pooler.Return(_pooledObject);
    }
}
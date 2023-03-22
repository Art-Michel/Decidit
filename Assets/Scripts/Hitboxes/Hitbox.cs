using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    [Foldout("Properties")]
    [SerializeField] protected LayerMask _shouldCollideWith;
    [Foldout("Properties")]
    [SerializeField] protected bool _canHitThroughWalls;
    [Foldout("Properties")]
    [HideIf("_canHitThroughWalls")][SerializeField] protected LayerMask _shouldNotHitThrough;
    [Foldout("Properties")]
    [SerializeField] protected bool _canMultiHit = false;
    [Foldout("Properties")]
    [SerializeField] private bool _shouldSplashBloodOnHit = false;
    [Foldout("Properties")]
    [SerializeField] private bool _canCriticalHit = true;

    [Foldout("Stats")]
    [SerializeField] protected float _radius = .2f;
    [Foldout("Stats")]
    [SerializeField] protected int _damage = 10;
    [Foldout("Stats")]
    [SerializeField] protected float _knockbackForce = 10f;
    [Foldout("Stats")]
    [SerializeField] protected Vector3 _knockbackAngle = Vector3.zero;
    [Foldout("Stats")]
    [ShowIf("_canMultiHit")][SerializeField] protected float _delayBetweenHits = 0f;

    public Dictionary<Transform, float> Blacklist { get; set; }

    protected virtual void Awake()
    {
        Blacklist = new Dictionary<Transform, float>();
    }

    private void OnEnable()
    {
        ClearBlacklist();
    }

    private void OnDrawGizmos()
    {
        if (this.enabled)
        {

            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }

    protected virtual void Update()
    {
        CheckForCollision();
        if (_canMultiHit) UpdateBlackList();
    }

    protected virtual void CheckForCollision()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, _radius, _shouldCollideWith))
            if (!AlreadyHit(collider.transform.parent))
            {
                //if that hitbox can hit through walls.
                if (_canHitThroughWalls)
                    Hit(collider.transform);
                else if (!Physics.Raycast(transform.position, (collider.transform.position - transform.position).normalized, _radius, _shouldNotHitThrough))
                    Hit(collider.transform);
            }
    }

    protected bool AlreadyHit(Transform target)
    {
        if (_canMultiHit)
        {
            if (Blacklist.ContainsKey(target))
                return Blacklist[target] > 0;
            else
                return false;
        }
        else
            return Blacklist.ContainsKey(target);
    }

    protected virtual void Hit(Transform targetCollider)
    {
        //Debug.Log(transform.name + " hit " + target.transform.name);
        if (targetCollider.parent.TryGetComponent<Health>(out Health health))
        {
            if (_shouldSplashBloodOnHit)
            {
                if (targetCollider.CompareTag("WeakHurtbox") && _canCriticalHit)
                    health.TakeCriticalDamage(_damage, transform.position, -transform.forward);
                else
                    health.TakeDamage(_damage, transform.position, -transform.forward);
            }
            else
            {
                if (targetCollider.CompareTag("WeakHurtbox") && _canCriticalHit)
                    health.TakeCriticalDamage(_damage);
                else
                    health.TakeDamage(_damage);
            }

            if (_knockbackForce > 0f)
            {
                //direction
                Vector3 direction;
                if (_knockbackAngle == Vector3.zero)
                    direction = (targetCollider.position - transform.position);
                else
                    direction = MakeDirectionRelative(_knockbackAngle.normalized);

                //weaker over distance
                //float force = _knockbackForce * Mathf.InverseLerp(_radius, 2, direction.magnitude);

                //apply knockback
                health.Knockback(direction.normalized * _knockbackForce);
            }
        }
        Blacklist.Add(targetCollider.parent, _delayBetweenHits);
    }

    Vector3 MakeDirectionRelative(Vector3 direction)
    {
        Vector3 that;
        that = transform.right * direction.x;
        that += transform.up * direction.y;
        that += transform.forward * direction.z;

        return that;
    }

    protected void UpdateBlackList()
    {
        if (Blacklist.Count > 0)
        {
            Transform[] keys = Blacklist.Keys.ToArray();
            foreach (Transform key in keys)
            {
                Blacklist[key] -= Time.deltaTime;
                if (Blacklist[key] <= 0)
                    Blacklist.Remove(key);
            }
        }
    }

    protected void ClearBlacklist()
    {
        Blacklist.Clear();
    }
}
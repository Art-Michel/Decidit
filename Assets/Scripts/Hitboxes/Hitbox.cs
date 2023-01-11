using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    [SerializeField] protected LayerMask _shouldCollideWith;
    [SerializeField] protected float _radius = .2f;
    [SerializeField] protected int _damage = 10;

    public Dictionary<Transform, float> Blacklist { get; set; }
    [SerializeField] protected bool _canMultiHit = false;
    [ShowIf("_canMultiHit")][SerializeField] protected float _delayBetweenHits = 0f;
    // [SerializeField] protected float _targetInvulnerability;

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
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _radius);
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
                Hit(collider.transform);
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

    protected void Hit(Transform targetCollider)
    {
        //Debug.Log(transform.name + " hit " + target.transform.name);
        if (targetCollider.parent.TryGetComponent<Health>(out Health health))
        {
            if (targetCollider.CompareTag("WeakHurtbox"))
                health.TakeCriticalDamage(_damage);
            else
                health.TakeDamage(_damage);
            Blacklist.Add(targetCollider.parent, _delayBetweenHits);
        }
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
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

    protected Dictionary<Transform, float> _blacklist;
    [SerializeField] protected bool _canMultiHit = false;
    [ShowIf("_canMultiHit")][SerializeField] protected float _delayBetweenHits = 0f;
    // [SerializeField] protected float _targetInvulnerability;

    private void Awake()
    {
        _blacklist = new Dictionary<Transform, float>();
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
            if (_blacklist.ContainsKey(target))
                return _blacklist[target] > 0;
            else
                return false;
        }
        else
            return _blacklist.ContainsKey(target);
    }

    protected void Hit(Transform targetCollider)
    {
        //Debug.Log(transform.name + " hit " + target.transform.name);
        if (targetCollider.CompareTag("WeakHurtbox"))
            targetCollider.parent.GetComponent<Health>().TakeCriticalDamage(_damage);
        else
            targetCollider.parent.GetComponent<Health>().TakeDamage(_damage);
        _blacklist.Add(targetCollider.parent, _delayBetweenHits);
    }

    protected void UpdateBlackList()
    {
        if (_blacklist.Count > 0)
        {
            Transform[] keys = _blacklist.Keys.ToArray();
            foreach (Transform key in keys)
            {
                _blacklist[key] -= Time.deltaTime;
                if (_blacklist[key] <= 0)
                    _blacklist.Remove(key);
            }
        }
    }

    private void ClearBlacklist()
    {
        _blacklist.Clear();
    }
}
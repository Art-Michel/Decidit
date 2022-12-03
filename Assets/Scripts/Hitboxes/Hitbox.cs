using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    [SerializeField] protected LayerMask _shouldCollideWith;
    [SerializeField] protected float _radius = .2f;
    [SerializeField] protected float _multiHitMaxCooldown;
    [SerializeField] protected int _damage;

    Dictionary<Transform, float> _blacklist;

    void Awake()
    {
        _blacklist = new Dictionary<Transform, float>();
    }

    void OnEnable()
    {
        ClearBlacklist();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _radius);
    }

    protected virtual void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _shouldCollideWith);
        if (colliders != null)
        {
            foreach (Collider collider in colliders)
                CheckForHit(collider.transform);
        }
    }

    void Update()
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

    protected void CheckForHit(Transform target)
    {
        if (!_blacklist.ContainsKey(target))
        {
            Hit(target);
            _blacklist.Add(target, _multiHitMaxCooldown);
        }
    }

    private void Hit(Transform target)
    {
        Debug.Log(transform.name + " hit " + target.transform.name);
        target.GetComponent<Health>().TakeDamage(_damage);
    }

    void ClearBlacklist()
    {
        _blacklist.Clear();
    }
}
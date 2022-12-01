using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    [SerializeField] protected LayerMask _shouldCollideWith;
    [SerializeField] protected float _radius = .2f;

    List<Transform> _blacklist;

    void Awake()
    {
        _blacklist = new List<Transform>();
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
                Hit(collider.transform);
        }
    }

    protected void Hit(Transform target)
    {
        if (!_blacklist.Contains(target))
        {
            Debug.Log(transform.name + " hit " + target.transform.name);
            target.GetComponent<Health>().TakeDamage();
            _blacklist.Add(target);
        }
    }

    void ClearBlacklist()
    {
        _blacklist.Clear();
    }
}
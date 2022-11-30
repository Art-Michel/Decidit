using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    [SerializeField] LayerMask _shouldCollideWith;
    [SerializeField] float _radius;

    List<Collider> _blacklist;

    void Awake()
    {
        _blacklist = new List<Collider>();
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
            {
                if (!_blacklist.Contains(collider))
                    Hit(collider);
            }
        }
    }

    protected virtual void Hit(Collider collider)
    {
        Debug.Log("hit " + collider.transform.name);
        collider.GetComponent<Health>().TakeDamage();
        _blacklist.Add(collider);
    }
}
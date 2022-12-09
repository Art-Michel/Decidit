using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    protected Pooler _pooler;

    public virtual void Init(Pooler pooler)
    {
        _pooler = pooler;
        transform.SetParent(pooler.transform);
        gameObject.SetActive(false);
    }
}

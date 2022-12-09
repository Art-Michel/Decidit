using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public Pooler Pooler { get; private set; }

    public virtual void Init(Pooler pooler, bool shouldBeParented)
    {
        Pooler = pooler;
        if (shouldBeParented)
            transform.SetParent(pooler.transform);
        gameObject.SetActive(false);
    }
}

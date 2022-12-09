using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    [SerializeField] protected GameObject _prefab;
    [SerializeField] int _initialCount = 4;
    [SerializeField] bool _shouldBeParent;
    Queue<PooledObject> _prefabs;

    void Awake()
    {
        _prefabs = new Queue<PooledObject>();
    }

    void Start()
    {
        for (int i = 0; i < _initialCount; i++)
        {
            _prefabs.Enqueue(Create());
        }
    }

    public PooledObject Get()
    {
        if (_prefabs.Count > 0)
        {
            PooledObject obj = _prefabs.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            PooledObject obj = Create();
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    public void Return(PooledObject obj)
    {
        _prefabs.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }

    protected virtual PooledObject Create()
    {
        GameObject obj = Instantiate(_prefab);
        PooledObject pooled = obj.GetComponent<PooledObject>();
        pooled.Init(this, _shouldBeParent);
        return pooled;
    }
}

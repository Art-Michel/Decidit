using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    List<Transform> _children;
    const float _delay = 0.1f;
    float _t = 0.0f;
    float _target = 0.0f;
    int _nextOneToSpawn = 0;

    void Awake()
    {
        // _children = new List<Transform>();
        foreach (Transform tr in transform)
        {
            foreach (Transform tr2 in tr)
                _children.Add(tr2);
        }
    }

    void Update()
    {
        _t += Time.deltaTime;
        if (_t >= _target)
            SpawnOneByOne();
    }

    void OnEnable()
    {
        SpawnOneByOne();
    }

    private void SpawnOneByOne()
    {
        _children[_nextOneToSpawn].gameObject.SetActive(true);
        _target += _delay;
        _nextOneToSpawn++;
        if (_nextOneToSpawn >= _children.Count)
            this.enabled = false;
    }
}

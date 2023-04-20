using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FugueMaladeShot : PooledObject
{
    [SerializeField] TrailRenderer[] _trails;

    public Queue<Transform> Targets;
    private Transform _currentTarget;

    [SerializeField] float _speed;
    float _currentLerpT;
    Vector3 _startingPoint;

    void Awake()
    {
        Targets = new Queue<Transform>();
    }

    public void Setup(List<EnemyHealth> enemies, Vector3 position)
    {
        enemies = enemies.OrderBy(_ => (_.transform.position - position).magnitude).ToList();

        foreach (EnemyHealth enemy in enemies)
            Targets.Enqueue(enemy.transform);

        _currentLerpT = 0.0f;
        _startingPoint = position;
        transform.position = position;

        foreach (TrailRenderer trail in _trails)
        {
            trail.emitting = true;
            trail.Clear();
        }
    }

    private void Update()
    {
        if (_currentTarget == null)
        {
            Next();
            return;
        }

        Move();
    }

    private void Move()
    {
        _currentLerpT += (Time.deltaTime / (_startingPoint - _currentTarget.position).magnitude) * _speed;
        transform.position = Vector3.Lerp(_startingPoint, _currentTarget.position, _currentLerpT);
        transform.forward = (_startingPoint - _currentTarget.position).normalized;
        if (_currentLerpT >= 1)
            Next();
    }

    private void Next()
    {
        if (Targets.Count <= 0)
        {
            Stop();
            return;
        }
        else
        {
            _currentTarget = Targets.Dequeue();
            _startingPoint = transform.position;
            _currentLerpT = 0.0f;
        }
    }

    private void Stop()
    {
        foreach (TrailRenderer trail in _trails)
        {
            trail.Clear();
            trail.emitting = false;
        }
        Pooler.Return(this);
    }
}

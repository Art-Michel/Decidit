using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FugueMaladeShot : FugueProjectile
{
    float _t;
    Vector3 _p0;
    Vector3 _p1;
    Vector3 _p2;
    Transform _target;

    public override void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection)
    {
        base.Setup(position, direction, cameraDirection);

        SetupBezier(position, direction);

        //Position Bezier2 à forward * 10
        //Look for enemy
    }

    private void SetupBezier(Vector3 position, Vector3 direction)
    {
        _t = 0;
        _p0 = position;
        _p1 = position + direction * (Vector3.Distance(_p0, _p2) / 2);

        UpdateTarget();
    }

    protected override void Move()
    {
        if (_t < 1)
        {
            _t += Time.deltaTime / Vector3.Distance(_p0, _p2) * _speed;
            Vector3 wantedPos = LerpPositionBezier();
            transform.position = wantedPos;
            Direction = (transform.position - _lastFramePosition).normalized;
        }
        else
        {
            StartDisappearing();
        }
    }

    protected override void Update()
    {
        UpdateTarget();
        base.Update();
    }

    private void UpdateTarget()
    {
        if (_target != null)
        {
            _p2 = _target.position;
        }
        else if (Synergies.Instance.Hospital.Count > 0)
        {
            _target = Synergies.Instance.Hospital[0].transform;
        }
        else
        {
            StartDisappearing();
        }
    }

    protected override void Bounce(RaycastHit hit)
    {
        SoundManager.Instance.PlaySound(_bounceSoundPath, 1, gameObject);
        Debug.Log("Bounce! (rebondis)");

        SetupBezier(transform.position, Vector3.Reflect(Direction, hit.normal));
    }

    private Vector3 LerpPositionBezier()
    {
        //(1-t)^2*P0 + 2(1-t)tP1 + t^2*P2
        //  u           u
        //   uu*P0 + 2 *u * t* P1 + tt * P2
        float u = 1 - _t;
        float tt = _t * _t;
        float uu = u * u;

        Vector3 point = uu * _p0;
        point += 2 * u * _t * _p1;
        point += tt * _p2;
        return point;
    }
}
//Inherited from PooledObject
// [SerializeField] TrailRenderer[] _trails;

// public Queue<Transform> Targets;
// private Transform _currentTarget;

// [SerializeField] float _speed;
// float _currentLerpT;
// Vector3 _startingPoint;

// void Awake()
// {
//     Targets = new Queue<Transform>();
// }

// public void Setup(List<EnemyHealth> enemies, Vector3 position)
// {
//     enemies = enemies.OrderBy(_ => (_.transform.position - position).magnitude).ToList();

//     foreach (EnemyHealth enemy in enemies)
//         Targets.Enqueue(enemy.transform);

//     _currentLerpT = 0.0f;
//     _startingPoint = position;
//     transform.position = position;

//     foreach (TrailRenderer trail in _trails)
//     {
//         trail.emitting = true;
//         trail.Clear();
//     }
// }

// private void Update()
// {
//     if (_currentTarget == null)
//     {
//         Next();
//         return;
//     }

//     Move();
// }

// private void Move()
// {
//     _currentLerpT += (Time.deltaTime / (_startingPoint - _currentTarget.position).magnitude) * _speed;
//     transform.position = Vector3.Lerp(_startingPoint, _currentTarget.position, _currentLerpT);
//     transform.forward = (_startingPoint - _currentTarget.position).normalized;
//     if (_currentLerpT >= 1)
//     {
//         Next();
//     }
// }

// private void Next()
// {
//     if (Targets.Count <= 0)
//     {
//         Stop();
//         return;
//     }
//     else
//     {
//         _currentTarget = Targets.Dequeue();
//         _startingPoint = transform.position;
//         _currentLerpT = 0.0f;
//     }
// }

// private void Stop()
// {
//     foreach (TrailRenderer trail in _trails)
//     {
//         trail.Clear();
//         trail.emitting = false;
//     }
//     Pooler.Return(this);
// }

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
    [SerializeField] AnimationCurve _speedCurve;

    public void Setup(Vector3 position, Vector3 direction, Vector3 cameraDirection, bool centered)
    {
        base.Setup(position, direction, cameraDirection);

        FindNewTarget();
        SetupBezier(position, direction);

        _centered = centered;

        //Position Bezier2 à forward * 10
        //Look for enemy
        SoundManager.Instance.PlaySound("event:/SFX_Controller/Synergies/AragonOnMuse/SoundOnHit", 1f, gameObject);
    }

    private void SetupBezier(Vector3 position, Vector3 direction)
    {
        _t = 0;
        _p0 = position;
        _p1 = position + (direction * Vector3.Distance(_p0, _p2));

    }

    private void FindNewTarget()
    {
        Vector3 startingPoint;
        if (_centered)
            startingPoint = transform.position;
        else
            startingPoint = transform.position + Direction * 10;

        EnemyHealth[] newTargets = Synergies.Instance.Hospital.OrderBy(h => (Vector3.Distance(h.transform.position, startingPoint))).ToArray();

        _target = newTargets[0].transform;
        _p2 = _target.position;
    }

    private void UpdateTarget()
    {
        if (_target != null)
        {
            _p2 = _target.position;
        }
        else if (Synergies.Instance.Hospital.Count > 0)
        {
            FindNewTarget();
            SetupBezier(transform.position, Direction);
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

    protected override void Move()
    {
        if (_t < 1)
        {
            _t += (Time.deltaTime * _speed) / Vector3.Distance(_p0, _p2);
            Vector3 wantedPos = LerpPositionBezier();
            transform.position = wantedPos;
            Direction = (transform.position - _lastFramePosition).normalized;
        }
        else if (!_isDisappearing)
        {
            StartDisappearing();
        }
    }

    protected override void Bounce(RaycastHit hit)
    {
        SoundManager.Instance.PlaySound(_bounceSoundPath, 1, gameObject);

        SetupBezier(transform.position, Vector3.Reflect(Direction, hit.normal));
    }

    private Vector3 LerpPositionBezier()
    {
        //(1-t)^2*P0 + 2(1-t)tP1 + t^2*P2
        //  u           u
        //   uu*P0 + 2 *u * t* P1 + tt * P2
        float u = 1 - _speedCurve.Evaluate(_t);
        float tt = _speedCurve.Evaluate(_t) * _speedCurve.Evaluate(_t);
        float uu = u * u;

        Vector3 point = uu * _p0;
        point += 2 * u * _speedCurve.Evaluate(_t) * _p1;
        point += tt * _p2;
        return point;
    }
}
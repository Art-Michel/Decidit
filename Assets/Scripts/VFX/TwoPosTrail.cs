using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TwoPosTrail : PooledObject
{
    [SerializeField] Transform _startObj;
    [SerializeField] Transform _endObj;
    [SerializeField] VisualEffect _vfx;
    private float _cd;

    public void SetPos(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        _endObj.localPosition = endPos;
    }

    public void Play()
    {
        _vfx.Play();
        _cd = .4f;
    }

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd <= 0f)
            this.Pooler.Return(this);
    }
}

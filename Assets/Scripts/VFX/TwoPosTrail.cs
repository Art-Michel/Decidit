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
        _vfx.SetVector3("Start_Pos", startPos);
        _vfx.SetVector3("End_Pos", endPos);
        transform.position = Vector3.zero;
        _startObj.position = startPos;
        _endObj.position = endPos;
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

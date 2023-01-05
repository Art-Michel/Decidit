using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TwoPosTrail : PooledObject
{
    [SerializeField] Transform _startObj;
    [SerializeField] Transform _endObj;
    [SerializeField] VisualEffect _vfx;
    public void SetPos(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        _endObj.localPosition = endPos;
    }
    public void Play()
    {
        _vfx.Play();
    }
}

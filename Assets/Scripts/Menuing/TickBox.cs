using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TickBox : MonoBehaviour
{
    [SerializeField] Image _tick;
    [SerializeField] bool _b;

    public void Press()
    {
        _b = !_b;

        if (_b)
            _tick.color = new Color(_tick.color.r, _tick.color.g, _tick.color.b, 0.9f);
        else
            _tick.color = new Color(_tick.color.r, _tick.color.g, _tick.color.b, 0.0f);
    }
}

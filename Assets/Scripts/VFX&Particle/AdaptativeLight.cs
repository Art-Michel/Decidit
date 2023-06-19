using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptativeLight : MonoBehaviour
{
    private Light _light;
    private float _initialLightStrength;
    [SerializeField] AnimationCurve _curve;
    [SerializeField] float _minDistance;
    [SerializeField] float _maxDistance;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Start()
    {
        _initialLightStrength = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, Player.Instance.transform.position);
        float proportionalDist = Mathf.InverseLerp(_minDistance, _maxDistance, dist);
        _light.intensity = _initialLightStrength * _curve.Evaluate(proportionalDist);
    }
}

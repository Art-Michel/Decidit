using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float _speed;

    void Update()
    {
        transform.Rotate(Vector3.forward * _speed * Time.deltaTime);
    }
}

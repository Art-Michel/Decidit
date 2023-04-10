using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] Volume _blurPostProcessVolume;

    private void Start()
    {
        _blurPostProcessVolume.gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime, Space.World);
    }
}

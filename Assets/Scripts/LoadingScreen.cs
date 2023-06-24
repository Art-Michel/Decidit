using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Sprite[] _spr;
    [SerializeField] Image _image;

    void OnEnable()
    {
        _image.sprite = _spr[Random.Range(0, _spr.Length)];
    }
}

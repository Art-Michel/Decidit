using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPoint : MonoBehaviour
{
    [SerializeField] Slider _slider;

    public void AddValueToSlider(float i)
    {
        _slider.value += i;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _valueTextMesh;

    public void AdjustValue(float i)
    {
        _valueTextMesh.text = (Mathf.Round(i * 10) / 10).ToString();
    }
}

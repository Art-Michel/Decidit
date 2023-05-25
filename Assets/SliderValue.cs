using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _valueTextMesh;

    public void AdjustValue(float i)
    {
        _valueTextMesh.text = i.ToString();
    }
}

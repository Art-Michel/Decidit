using UnityEngine;
using TMPro;

public class DisplayTimer : MonoBehaviour
{
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = System.TimeSpan.FromSeconds((int)TimerManager.Instance.time).ToString();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DisplayTimer : MonoBehaviour
{
    TextMeshProUGUI txt;
    PlayerInputMap _inputs;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        _inputs = new PlayerInputMap();
        _inputs.MenuNavigation.Score.started += _ => EnableScore();
    }

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();

        if (txt.gameObject.name !="DisplayTimerEndGame")
            txt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = System.TimeSpan.FromSeconds((int)TimerManager.Instance.time).ToString();
    }

    private void EnableScore()
    {
        if (!TimerManager.Instance.endGame)
            txt.enabled = !txt.enabled;
        else
            txt.enabled = true;
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable()
    {
        _inputs.Disable();
    }
}

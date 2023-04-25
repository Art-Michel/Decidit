using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        MenuManager.Instance.StartMenuing();
        MenuManager.Instance.StartUnfading(1);
    }
}

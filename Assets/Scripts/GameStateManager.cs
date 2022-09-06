using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    float _cooldown = 3f;
    bool is30 = false;

    void Update()
    {
        _cooldown -= Time.deltaTime;
        if (_cooldown <= 0)
        {
            ChangeTime();
        }
    }

    void ChangeTime()
    {
        _cooldown = 3f;
        if (is30)
        {
            is30 = false;
            Application.targetFrameRate = 180;
        }
        else
        {
            is30 = true;
            Application.targetFrameRate = 30;
        }
    }
}

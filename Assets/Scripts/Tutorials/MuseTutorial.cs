using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseTutorial : Tutorial
{
    protected override void Enable()
    {
        base.Enable();
        MenuManager.Instance.OpenSynergyMuseTutorial();
    }
}

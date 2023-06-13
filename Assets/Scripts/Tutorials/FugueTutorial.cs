using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FugueTutorial : Tutorial
{
    protected override void Enable()
    {
        base.Enable();
        MenuManager.Instance.OpenSynergyFugueTutorial();
    }
}

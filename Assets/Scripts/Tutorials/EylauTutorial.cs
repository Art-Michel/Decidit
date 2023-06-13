using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EylauTutorial : Tutorial
{
    protected override void Enable()
    {
        base.Enable();
        MenuManager.Instance.OpenSynergyEylauTutorial();
    }
}

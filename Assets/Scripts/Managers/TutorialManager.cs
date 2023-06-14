using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : LocalManager<TutorialManager>
{
    [SerializeField] Tutorial _fugueTutorial;
    [SerializeField] Tutorial _museTutorial;
    [SerializeField] Tutorial _eylauTutorial;
    [SerializeField] Tutorial _movementTutorial;
    [SerializeField] Tutorial _jumpingTutorial;
    [SerializeField] Tutorial _walljumpingTutorial;
    [SerializeField] Tutorial _gunTutorial;
    [SerializeField] Tutorial _armTutorial;
    [SerializeField] Tutorial _healthTutorial;

    public void StartTutorial(string tutorial)
    {
        if (!PlayerManager.ShouldTutorial)
            return;

        switch (tutorial)
        {
            case "fugue":
                _fugueTutorial.Enable();
                break;
            case "muse":
                _museTutorial.Enable();
                break;
            case "eylau":
                _eylauTutorial.Enable();
                break;
            case "gun":
                _gunTutorial.Enable();
                break;
            case "arm":
                _armTutorial.Enable();
                break;
            case "move":
                _movementTutorial.Enable();
                break;
            case "jump":
                _jumpingTutorial.Enable();
                break;
            case "walljump":
                _walljumpingTutorial.Enable();
                break;
            case "health":
                _healthTutorial.Enable();
                break;
        }
    }
}
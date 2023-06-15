using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : LocalManager<TutorialManager>
{
    [SerializeField] Tutorial _movementTutorial;
    [SerializeField] Tutorial _jumpingTutorial;
    [SerializeField] Tutorial _walljumpingTutorial;
    [SerializeField] Tutorial _gunTutorial;
    [SerializeField] Tutorial _armTutorial;
    [SerializeField] Tutorial _healthTutorial;
    [SerializeField] Tutorial _fugueTutorial;
    [SerializeField] Tutorial _museTutorial;
    [SerializeField] Tutorial _eylauTutorial;

    Dictionary<Tutorials, Tutorial> _tutoDictionary;
    public static Dictionary<Tutorials, bool> _tutorialWasSeen;

    public enum Tutorials
    {
        Move,
        Jump,
        Walljump,
        Gun,
        Arm,
        Health,
        FugueSynergy,
        MuseSynergy,
        EylauSynergy,
    }

    protected override void Awake()
    {
        base.Awake();

        _tutoDictionary = new Dictionary<Tutorials, Tutorial>()
        {
            {Tutorials.Move, _movementTutorial},
            {Tutorials.Jump, _jumpingTutorial},
            {Tutorials.Walljump, _walljumpingTutorial},
            {Tutorials.Gun, _gunTutorial},
            {Tutorials.Arm, _armTutorial},
            {Tutorials.Health, _healthTutorial},
            {Tutorials.FugueSynergy, _fugueTutorial},
            {Tutorials.MuseSynergy, _museTutorial},
            {Tutorials.EylauSynergy, _eylauTutorial},
        };
    }

    public void StartTutorial(Tutorials tutorial)
    {
        // if (!PlayerManager.ShouldTutorial)
        //     return;

        if (Player.Instance.CurrentArm == PlayerManager.Instance.Arms[0] && tutorial == Tutorials.Arm)
            return;

        //TODO Lucas Son tuto
        _tutoDictionary[tutorial].Enable();
    }

    public void StopTutorial(Tutorials tutorial)
    {
        // if (!PlayerManager.ShouldTutorial)
        //     return;

        _tutoDictionary[tutorial].Disable();
    }

}
using System.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
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
    static Dictionary<Tutorials, bool> _tutorialWasSeen = new Dictionary<Tutorials, bool>
    {
        {Tutorials.Move, false},
        {Tutorials.Jump, false},
        {Tutorials.Walljump, false},
        {Tutorials.Gun, false},
        {Tutorials.Arm, false},
        {Tutorials.Health, false},
        {Tutorials.FugueSynergy, false},
        {Tutorials.MuseSynergy, false},
        {Tutorials.EylauSynergy, false},
    };

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
        Synergy
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

        if (tutorial == Tutorials.Arm)
            if (!ArmTutoCheck() || SynergyTutoCheck())
                return;
        if (tutorial == Tutorials.Synergy)
            if (!SynergyTutoCheck())
                return;
            else
            {
                StartSynergyTutorial();
                return;
            }

        if (_tutorialWasSeen[tutorial])
            return;

        //TODO Lucas Son tuto
        _tutoDictionary[tutorial].Enable();
        _tutorialWasSeen[tutorial] = true;
    }

    private bool SynergyTutoCheck()
    {
        bool cond;
        cond = Player.Instance.CurrentArm.gameObject != PlayerManager.Instance.Arms[0];
        cond = cond && Player.Instance.CurrentGun.gameObject != PlayerManager.Instance.Guns[0];
        return cond;
    }

    private void StartSynergyTutorial()
    {
        //TODO Lucas Son tuto
        switch (Player.Instance.CurrentArm.Chant)
        {
            case Synergies.Chants.ARAGON:
                if (_tutorialWasSeen[Tutorials.FugueSynergy])
                    break;
                _tutoDictionary[Tutorials.FugueSynergy].Enable();
                _tutorialWasSeen[Tutorials.FugueSynergy] = true;
                break;
            case Synergies.Chants.EYLAU:
                if (_tutorialWasSeen[Tutorials.EylauSynergy])
                    break;
                _tutoDictionary[Tutorials.EylauSynergy].Enable();
                _tutorialWasSeen[Tutorials.EylauSynergy] = true;
                break;
            case Synergies.Chants.MUSE:
                if (_tutorialWasSeen[Tutorials.MuseSynergy])
                    break;
                _tutoDictionary[Tutorials.MuseSynergy].Enable();
                _tutorialWasSeen[Tutorials.MuseSynergy] = true;
                break;
        }
    }

    private bool ArmTutoCheck()
    {
        bool cond;
        cond = Player.Instance.CurrentArm.gameObject != PlayerManager.Instance.Arms[0];
        cond = cond && Player.Instance.CurrentGun.gameObject == PlayerManager.Instance.Guns[0];
        return cond;
    }

    public void StopTutorial(Tutorials tutorial)
    {
        if (tutorial == Tutorials.Synergy)
            if (!SynergyTutoCheck())
                return;
            else
            {
                StopSynergyTutorial();
                return;
            }

        _tutoDictionary[tutorial].Disable();
    }

    private void StopSynergyTutorial()
    {
        switch (Player.Instance.CurrentArm.Chant)
        {
            case Synergies.Chants.ARAGON:
                if (_tutorialWasSeen[Tutorials.FugueSynergy])
                    break;
                _tutoDictionary[Tutorials.FugueSynergy].Disable();
                break;

            case Synergies.Chants.EYLAU:
                if (_tutorialWasSeen[Tutorials.EylauSynergy])
                    break;
                _tutoDictionary[Tutorials.EylauSynergy].Disable();
                break;

            case Synergies.Chants.MUSE:
                if (_tutorialWasSeen[Tutorials.MuseSynergy])
                    break;
                _tutoDictionary[Tutorials.MuseSynergy].Disable();
                break;
        }
    }

    public void ResetTutorials()
    {
        foreach (Tutorials tutorial in _tutorialWasSeen.Keys)
        {
            _tutorialWasSeen[tutorial] = false;
        }
    }

}
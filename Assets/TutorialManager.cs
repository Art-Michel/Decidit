using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : LocalManager<TutorialManager>
{
    [SerializeField] GameObject _fugueTutorial;
    [SerializeField] GameObject _museTutorial;
    [SerializeField] GameObject _eylauTutorial;


    public void StartSynergyTutorial(Synergies.Chants chant)
    {
        switch (chant)
        {
            case Synergies.Chants.ARAGON:
                _fugueTutorial.SetActive(true);
                break;
            case Synergies.Chants.MUSE:
                _museTutorial.SetActive(true);
                break;
            case Synergies.Chants.EYLAU:
                _eylauTutorial.SetActive(true);
                break;
        }
    }
}

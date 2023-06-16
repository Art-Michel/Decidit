using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] TutorialManager.Tutorials _tutorialToOpen;
    [SerializeField] bool _wasDisabled = false;
    [SerializeField] bool _wasStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TutorialManager.Instance.StartTutorial(_tutorialToOpen))
                _wasStarted = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _wasStarted)
        {
            TutorialManager.Instance.StopTutorial(_tutorialToOpen);

            _wasDisabled = true;
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (!_wasDisabled && _wasStarted)
            TutorialManager.Instance.StopTutorial(_tutorialToOpen);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] TutorialManager.Tutorials _tutorialToOpen;
    bool _wasDisabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.StartTutorial(_tutorialToOpen);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.StopTutorial(_tutorialToOpen);

            _wasDisabled = true;
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (!_wasDisabled)
            TutorialManager.Instance.StopTutorial(_tutorialToOpen);
    }
}

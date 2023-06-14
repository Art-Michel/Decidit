using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] TutorialManager.Tutorials _tutorialToOpen;
    [SerializeField] bool _shouldReappear = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.StartTutorial(_tutorialToOpen);
            if (!_shouldReappear)
                gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.StopTutorial(_tutorialToOpen);
        }
    }
}

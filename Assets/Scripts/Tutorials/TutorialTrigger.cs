using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] TutorialManager.Tutorials _tutorialToOpen;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("started Tutorial");
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.StartTutorial(_tutorialToOpen);
            gameObject.SetActive(false);
        }
    }
}
